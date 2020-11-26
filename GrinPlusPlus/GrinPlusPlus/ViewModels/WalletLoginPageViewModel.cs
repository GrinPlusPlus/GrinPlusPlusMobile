using GrinPlusPlus.Api;
using Plugin.Fingerprint;
using Plugin.Fingerprint.Abstractions;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using Prism.Services.Dialogs;
using System;
using System.Threading;
using Xamarin.Essentials;

namespace GrinPlusPlus.ViewModels
{
    public class WalletLoginPageViewModel : ViewModelBase
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

        public DelegateCommand OpenWalletCommand => new DelegateCommand(OpenWallet);

        public WalletLoginPageViewModel(INavigationService navigationService, IDataProvider dataProvider, IDialogService dialogService, IPageDialogService pageDialogService)
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

        private async void OpenWallet()
        {
            try
            {
                ExceptionMessage = string.Empty;

                IsBusy = true;
                IsIdle = false;

                if (string.IsNullOrEmpty(Password))
                {
                    throw new Exception("Password can not be empty.");
                }
                var wallet = await DataProvider.DoLogin(Username, Password);
                if (!string.IsNullOrEmpty(wallet.Token))
                {
                    await SecureStorage.SetAsync("token", wallet.Token);
                    await SecureStorage.SetAsync("username", Username);
                    await SecureStorage.SetAsync("slatepack_address", wallet.SlatepackAdddress);
                    await SecureStorage.SetAsync("tor_address", wallet.TorAdddress);

                    if (await CrossFingerprint.Current.IsAvailableAsync(true))
                    {
                        _cancel = new CancellationTokenSource();
                        var dialogConfig = new AuthenticationRequestConfiguration("Grin++", $"Opening {Username.ToUpper()} Wallet")
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

                    var balance = await DataProvider.GetWalletBalance(await SecureStorage.GetAsync("token"));
                    if (balance != null)
                    {
                        Preferences.Set("balance_spendable", balance.Spendable.ToString());
                        Preferences.Set("balance_locked", balance.Locked.ToString());
                        Preferences.Set("balance_immature", balance.Immature.ToString());
                        Preferences.Set("balance_unconfirmed", balance.Unconfirmed.ToString());
                        Preferences.Set("balance_total", balance.Total.ToString());
                    }

                    await NavigationService.NavigateAsync("/SharedTransitionNavigationPage/DashboardCarouselPage", new NavigationParameters { { "wallet", Username } });
                }
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
    }
}
