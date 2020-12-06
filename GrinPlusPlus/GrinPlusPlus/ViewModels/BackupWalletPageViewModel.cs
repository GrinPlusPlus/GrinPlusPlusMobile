using GrinPlusPlus.Api;
using GrinPlusPlus.Resources;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using Prism.Services.Dialogs;
using System;
using Xamarin.Essentials;

namespace GrinPlusPlus.ViewModels
{
    public class BackupWalletPageViewModel : ViewModelBase
    {
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
                WalletSeed = await DataProvider.BackupWallet(Username, Password);
            }
            catch (Exception ex)
            {
                ExceptionMessage = ex.Message;
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
