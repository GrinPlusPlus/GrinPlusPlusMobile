using GrinPlusPlus.Api;
using GrinPlusPlus.Models;
using GrinPlusPlus.Resources;
using Plugin.Fingerprint;
using Plugin.Fingerprint.Abstractions;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Xamarin.Essentials;

namespace GrinPlusPlus.ViewModels
{
    public class WalletLoginPageViewModel : ViewModelBase
    {
        private string _username = string.Empty;
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

        private string _password = string.Empty;
        public string Password
        {
            get => _password.Trim();
            set => SetProperty(ref _password, value);
        }

        public DelegateCommand OpenWalletCommand => new DelegateCommand(OpenWallet);

        public DelegateCommand DeleteWalletCommand => new DelegateCommand(DeleteWalletC);

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
            IsBusy = true;

            try
            {
                var wallet = await DataProvider.DoLogin(Username, Password);

                await SecureStorage.SetAsync("token", wallet.Token);
                await SecureStorage.SetAsync("username", Username);
                await SecureStorage.SetAsync("slatepack_address", wallet.SlatepackAdddress);
                await SecureStorage.SetAsync("tor_address", wallet.TorAddress ?? string.Empty);

                Settings.IsLoggedIn = true;

                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await NavigationService.NavigateAsync("OpeningWalletPage");
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);

                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await PageDialogService.DisplayAlertAsync("Error", ex.Message, "OK");
                });
            }
            finally
            {
                IsBusy = false;
            }
        }

        async void DeleteWalletC()
        {
            IsBusy = true;

            var yes = AppResources.ResourceManager.GetString("Yes");
            var no = AppResources.ResourceManager.GetString("No");
            var confirm = AppResources.ResourceManager.GetString("Confirm");
            var areyousure = AppResources.ResourceManager.GetString("AreYouSure");

            var confirmation = await PageDialogService.DisplayAlertAsync(confirm, areyousure, yes, no);
            if (!confirmation)
            {
                return;
            }

            var deleteWallet = true;

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

                var result = await CrossFingerprint.Current.AuthenticateAsync(dialogConfig, _cancel.Token);

                if (!result.Authenticated)
                {
                    await PageDialogService.DisplayAlertAsync("Error", "Authentication Failed", "OK");
                    deleteWallet = false;
                }
            }

            if (!deleteWallet)
            {
                return;
            }

            try
            {
                await DataProvider.DeleteWallet(Username, Password);
                List<Account> accounts = await DataProvider.GetAccounts();
                if (accounts.Count == 0)
                {
                    await NavigationService.NavigateAsync("/NavigationPage/LoginPage");
                }
                else if (accounts.Count == 1)
                {
                    await NavigationService.NavigateAsync("/NavigationPage/WalletLoginPage", new NavigationParameters { { "username", accounts[0].Name } });
                }
                else
                {
                    IActionSheetButton[] buttons = new ActionSheetButton[accounts.Count];
                    for (int i = 0; i < accounts.Count; i++)
                    {
                        string account = accounts[i].Name;
                        buttons[i] = ActionSheetButton.CreateButton(account, () =>
                        {
                            MainThread.BeginInvokeOnMainThread(async () =>
                            {
                                await NavigationService.NavigateAsync("/NavigationPage/WalletLoginPage", new NavigationParameters { { "username", account } });
                            });

                        });
                    }
                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        await PageDialogService.DisplayActionSheetAsync(string.Empty, buttons);
                    });
                }
            }
            catch (Exception ex)
            {
                IsBusy = false;
                Debug.WriteLine(ex.Message);
                await PageDialogService.DisplayAlertAsync("Error", ex.Message, "OK");
            }
        }
    }
}
