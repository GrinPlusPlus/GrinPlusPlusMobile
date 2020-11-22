using GrinPlusPlus.Api;
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
    public class AccountPasswordDialogViewModel : ViewModelBase, IDialogAware
    {
        private CancellationTokenSource _cancel;

        private string _username;
        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        private string _password;
        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        public DelegateCommand CloseCommand { get; }

        public AccountPasswordDialogViewModel(INavigationService navigationService, IDataProvider dataProvider, IDialogService dialogService, IPageDialogService pageDialogService)
            : base(navigationService, dataProvider, dialogService, pageDialogService)
        {
            CloseCommand = new DelegateCommand(async () =>
            {
                try
                {
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

                        var balance = await dataProvider.GetWalletBalance(await SecureStorage.GetAsync("token"));
                        if (balance != null)
                        {
                            Preferences.Set("balance_spendable", balance.Spendable.ToString());
                            Preferences.Set("balance_locked", balance.Locked.ToString());
                            Preferences.Set("balance_immature", balance.Immature.ToString());
                            Preferences.Set("balance_unconfirmed", balance.Unconfirmed.ToString());
                            Preferences.Set("balance_total", balance.Total.ToString());
                        }

                        RequestClose(null);
                        await NavigationService.NavigateAsync("/SharedTransitionNavigationPage/DashboardCarouselPage", new NavigationParameters { { "wallet", Username } });
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            });
        }

        public event Action<IDialogParameters> RequestClose;

        public bool CanCloseDialog() => true;

        public void OnDialogClosed()
        {

        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            if (parameters.ContainsKey("username"))
            {
                Username = parameters.GetValue<string>("username");
            }
        }
    }
}
