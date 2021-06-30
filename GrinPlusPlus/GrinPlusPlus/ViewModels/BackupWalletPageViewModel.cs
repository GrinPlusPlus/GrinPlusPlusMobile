using GrinPlusPlus.Api;
using GrinPlusPlus.Resources;
using Plugin.Fingerprint;
using Plugin.Fingerprint.Abstractions;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using Prism.Services.Dialogs;
using System;
using System.Diagnostics;
using System.Threading;
using Xamarin.Essentials;

namespace GrinPlusPlus.ViewModels
{
    public class BackupWalletPageViewModel : ViewModelBase
    {
        private CancellationTokenSource _cancel;

        private string _exceptionMessage = "";
        public string ExceptionMessage
        {
            get { return _exceptionMessage; }
            set { SetProperty(ref _exceptionMessage, value.Trim()); }
        }

        private string _username = "";
        public string Username
        {
            get => _username.Trim();
            set => SetProperty(ref _username, value);
        }

        private bool _isBusy = false;
        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        private bool _isIdle = true;
        public bool IsIdle
        {
            get => _isIdle;
            set => SetProperty(ref _isIdle, value);
        }

        private string _password = "";
        public string Password
        {
            get => _password.Trim();
            set => SetProperty(ref _password, value);
        }

        private string _walletSeed = "";
        public string WalletSeed
        {
            get => _walletSeed.Trim();
            set => SetProperty(ref _walletSeed, value);
        }

        public DelegateCommand CopyWalletSeedCommand => new DelegateCommand(CopyWalletSeed);

        private async void CopyWalletSeed()
        {
            await Clipboard.SetTextAsync(WalletSeed);
        }

        public DelegateCommand BackupWalletCommand => new DelegateCommand(BackupWallet);

        private async void BackupWallet()
        {
            try
            {
                ExceptionMessage = string.Empty;

                IsBusy = true;
                IsIdle = false;

                if (string.IsNullOrEmpty(Password))
                {
                    throw new Exception(AppResources.ResourceManager.GetString("PasswordCanNotBeEmpty"));
                }

                var seed = await DataProvider.BackupWallet(Username, Password);

                if (await CrossFingerprint.Current.IsAvailableAsync(true))
                {
                    _cancel = new CancellationTokenSource();

                    var wallet = Username.ToUpper();
                    var message = AppResources.ResourceManager.GetString("ConfirmIdentity");

                    var dialogConfig = new AuthenticationRequestConfiguration(wallet, message)
                    {
                        CancelTitle = null,
                        FallbackTitle = null,
                        AllowAlternativeAuthentication = true
                    };

                    var result = await CrossFingerprint.Current.AuthenticateAsync(dialogConfig, _cancel.Token);

                    if (!result.Authenticated)
                    {
                        return;
                    }
                }

                WalletSeed = seed;
            }
            catch (Exception ex)
            {
                ExceptionMessage = ex.Message;

                Debug.WriteLine(ex.Message);
            }
            finally
            {
                IsBusy = false;
                IsIdle = true;
            }
        }

        public BackupWalletPageViewModel(INavigationService navigationService, IDataProvider dataProvider, IDialogService dialogService, IPageDialogService pageDialogService)
            : base(navigationService, dataProvider, dialogService, pageDialogService)
        {

        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            if (parameters.ContainsKey("username"))
            {
                Username = parameters.GetValue<string>("username");
            }
        }
    }
}
