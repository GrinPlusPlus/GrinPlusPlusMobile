using GrinPlusPlus.Api;
using GrinPlusPlus.Resources;
using Plugin.Fingerprint;
using Plugin.Fingerprint.Abstractions;
using Prism.Navigation;
using Prism.Services;
using Prism.Services.Dialogs;
using System;
using System.Diagnostics;
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
            Task.Factory.StartNew(async () =>
            {
                Wallet = await SecureStorage.GetAsync("username");
            });
        }

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            try
            {
                var balance = await DataProvider.GetWalletBalance(await SecureStorage.GetAsync("token").ConfigureAwait(false)).ConfigureAwait(false);

                Preferences.Set("balance_spendable", balance.Spendable);
                Preferences.Set("balance_locked", balance.Locked);
                Preferences.Set("balance_immature", balance.Immature);
                Preferences.Set("balance_unconfirmed", balance.Unconfirmed);
                Preferences.Set("balance_total", balance.Total);

                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    if (await CrossFingerprint.Current.IsAvailableAsync(true))
                    {
                        var _cancel = new CancellationTokenSource();

                        var message = AppResources.ResourceManager.GetString("ConfirmIdentity");

                        var dialogConfig = new AuthenticationRequestConfiguration("Fingerprint", message)
                        {
                            CancelTitle = null,
                            FallbackTitle = null,
                            AllowAlternativeAuthentication = true
                        };

                        var result = await CrossFingerprint.Current.AuthenticateAsync(dialogConfig, _cancel.Token);

                        if (!result.Authenticated)
                        {
                            ExceptionMessage = "Can't be authenticated";
                            Debug.WriteLine("Can't be authenticated");
                            Thread.Sleep(2000);
                            await Logout();
                            await NavigationService.GoBackToRootAsync();
                        }
                    }

                    SecureStorage.Remove("wallet_seed");
                    await NavigationService.NavigateAsync("/NavigationPage/WalletPage");
                }).ConfigureAwait(false) ;
            }
            catch (Exception ex)
            {
                ExceptionMessage = ex.Message;
                Debug.WriteLine(ex.Message);
                Thread.Sleep(2000);
                await Logout();
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    await NavigationService.GoBackToRootAsync();
                }).ConfigureAwait(false);
            }
        }

        async Task Logout()
        {
            try
            {
                await DataProvider.DoLogout(await SecureStorage.GetAsync("token").ConfigureAwait(false)).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ExceptionMessage = ex.Message;
                Debug.WriteLine(ex.Message);
            }

            SecureStorage.RemoveAll();

            Settings.IsLoggedIn = false;

            Preferences.Remove("balance_spendable");
            Preferences.Remove("balance_locked");
            Preferences.Remove("balance_immature");
            Preferences.Remove("balance_unconfirmed");
            Preferences.Remove("balance_total");
        }
    }
}
