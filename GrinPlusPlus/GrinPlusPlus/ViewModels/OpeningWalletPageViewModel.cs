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

            SecureStorage.Remove("wallet_seed"); // in case of a newly created wallet
        }

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            try
            {
                var balance = await DataProvider.GetWalletBalance(await SecureStorage.GetAsync("token"));

                Preferences.Set("balance_spendable", balance.Spendable);
                Preferences.Set("balance_locked", balance.Locked);
                Preferences.Set("balance_immature", balance.Immature);
                Preferences.Set("balance_unconfirmed", balance.Unconfirmed);
                Preferences.Set("balance_total", balance.Total);

                FingerprintAvailability availability = await CrossFingerprint.Current.GetAvailabilityAsync();

                if (availability.Equals(FingerprintAvailability.Available) && await CrossFingerprint.Current.IsAvailableAsync())
                {
                    var _cancel = new CancellationTokenSource();

                    var message = AppResources.ResourceManager.GetString("ConfirmIdentity");

                    var dialogConfig = new AuthenticationRequestConfiguration("Fingerprint", message)
                    {
                        CancelTitle = AppResources.ResourceManager.GetString("Cancel"),
                        FallbackTitle = null,
                        AllowAlternativeAuthentication = true
                    };

                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        var result = await CrossFingerprint.Current.AuthenticateAsync(dialogConfig, _cancel.Token);

                        if (!result.Authenticated)
                        {
                            await PageDialogService.DisplayAlertAsync("Error", "Authentication Failed", "OK");
                            await Logout();
                            await NavigationService.GoBackToRootAsync();
                        }
                        else
                        {
                            await NavigationService.NavigateAsync("/NavigationPage/WalletPage");
                        }
                    });
                }
                else
                {
                    await MainThread.InvokeOnMainThreadAsync(async () =>
                    {
                        await NavigationService.NavigateAsync("/NavigationPage/WalletPage");
                    });
                }
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
                });
            }
        }

        async Task Logout()
        {
            try
            {
                await DataProvider.DoLogout(await SecureStorage.GetAsync("token"));
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
