using GrinPlusPlus.Api;
using GrinPlusPlus.Resources;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using Prism.Services.Dialogs;
using System;
using System.Diagnostics;
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

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            if (parameters.ContainsKey("username"))
            {
                Username = parameters.GetValue<string>("username");
                Title = Username.ToUpper();
            }
            else
            {
                await NavigationService.GoBackToRootAsync();
            }
        }

        private async void OpenWallet()
        {
            SecureStorage.RemoveAll();
            IsBusy = true;

            try
            {
                var wallet = await DataProvider.DoLogin(Username, Password).ConfigureAwait(false);
                
                
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
                IsBusy = false;
                Debug.WriteLine(ex.Message);
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await PageDialogService.DisplayAlertAsync("Error", ex.Message, "OK");
                });
            }
        }

        private async void DeleteWalletC()
        {
            IsBusy = true;

            try
            {
                await DataProvider.DeleteWallet(Username, Password).ConfigureAwait(false);
                await NavigationService.GoBackToRootAsync();
            }
            catch (Exception ex)
            {
                IsBusy = false;
                Debug.WriteLine(ex.Message);
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await PageDialogService.DisplayAlertAsync("Error", ex.Message, "OK");
                });
            }
        }
    }
}
