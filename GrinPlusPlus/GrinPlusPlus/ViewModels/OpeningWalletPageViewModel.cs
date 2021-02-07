using GrinPlusPlus.Api;
using GrinPlusPlus.Resources;
using Plugin.Fingerprint;
using Plugin.Fingerprint.Abstractions;
using Prism.Navigation;
using Prism.Services;
using Prism.Services.Dialogs;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;


namespace GrinPlusPlus.ViewModels
{
    public class OpeningWalletPageViewModel : ViewModelBase
    {
        private string _wallet = string.Empty;
        public string Wallet
        {
            get { return _wallet; }
            set { SetProperty(ref _wallet, value); }
        }

        private string _exceptionMessage;
        public string ExceptionMessage
        {
            get { return _exceptionMessage; }
            set { SetProperty(ref _exceptionMessage, value); }
        }


        public OpeningWalletPageViewModel(INavigationService navigationService, IDataProvider dataProvider, IDialogService dialogService, IPageDialogService pageDialogService)
            : base(navigationService, dataProvider, dialogService, pageDialogService)
        {
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                try
                {
                    Wallet = (await SecureStorage.GetAsync("username")).ToUpper();
                    await GetWalletBalance();
                }
                catch (Exception ex)
                {
                    ExceptionMessage = ex.Message;
                    Thread.Sleep(3000);
                    await NavigationService.GoBackToRootAsync();
                }
            });
        }

        private async Task GetWalletBalance()
        {
            var balance = await DataProvider.GetWalletBalance(await SecureStorage.GetAsync("token"));

            if (await CrossFingerprint.Current.IsAvailableAsync(true))
            {
                var _cancel = new CancellationTokenSource();

                var message = AppResources.ResourceManager.GetString("ConfirmIdentity");

                var dialogConfig = new AuthenticationRequestConfiguration(Wallet, message)
                {
                    CancelTitle = null,
                    FallbackTitle = null,
                    AllowAlternativeAuthentication = true
                };

                var result = await CrossFingerprint.Current.AuthenticateAsync(dialogConfig, _cancel.Token);

                if (!result.Authenticated)
                {
                    Settings.IsLoggedIn = false;
                    await Logout();
                }
            }

            Preferences.Set("balance_spendable", balance.Spendable);
            Preferences.Set("balance_locked", balance.Locked);
            Preferences.Set("balance_immature", balance.Immature);
            Preferences.Set("balance_unconfirmed", balance.Unconfirmed);
            Preferences.Set("balance_total", balance.Total);

            Settings.IsLoggedIn = true;
            await NavigationService.NavigateAsync("/SharedTransitionNavigationPage/DashboardCarouselPage");
        }

        async Task Logout()
        {
            try
            {
                await DataProvider.DoLogout(await SecureStorage.GetAsync("token"));
                var torAddress = await SecureStorage.GetAsync("tor_address");
                if (!string.IsNullOrEmpty(torAddress))
                {
                    await DataProvider.DeleteONION(torAddress);
                    SecureStorage.Remove("tor_address");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Preferences.Clear();
            SecureStorage.RemoveAll();

            Settings.IsLoggedIn = false;
        }
    }
}
