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
                    await GetNodePreferences();
                    await GetWalletBalance();
                    Settings.IsLoggedIn = true;
                    await NavigationService.NavigateAsync("/SharedTransitionNavigationPage/DashboardCarouselPage");
                }
                catch (Exception ex)
                {
                    ExceptionMessage = ex.Message;
                    Thread.Sleep(2000);
                    Preferences.Clear();
                    SecureStorage.RemoveAll();
                    await NavigationService.GoBackToRootAsync();
                }
            });
        }

        private async Task GetNodePreferences()
        {
            var preferences = await DataProvider.GetNodeSettings();

            Settings.Confirmations = preferences.Confirmations;
            Settings.MinimumPeers = preferences.MinimumPeers;
            Settings.MaximumPeers = preferences.MaximumPeers;
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
        }

        async Task Logout()
        {
            try
            {
                await DataProvider.DoLogout(await SecureStorage.GetAsync("token"));
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
