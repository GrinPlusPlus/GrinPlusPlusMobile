using GrinPlusPlus.Api;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using Prism.Services.Dialogs;
using Xamarin.Essentials;

namespace GrinPlusPlus.ViewModels
{
    public class RestoreWalletPageViewModel : ViewModelBase
    {
        private string _username;
        public string Username
        {
            get { return _username; }
            set { SetProperty(ref _username, value.Trim()); }
        }

        private string _password;
        public string Password
        {
            get { return _password; }
            set { SetProperty(ref _password, value); }
        }

        private string _walletSeed;
        public string WalletSeed
        {
            get { return _walletSeed; }
            set { 
                SetProperty(ref _walletSeed, value.Trim()); 
                IsFormValid = IsUsernameValid && IsPasswordValid && value.Trim().Length > 0;
            }
        }

        private bool _isUsernameValid;
        public bool IsUsernameValid
        {
            get { return _isUsernameValid; }
            set
            {
                SetProperty(ref _isUsernameValid, value);
            }
        }

        private bool _isPasswordValid;
        public bool IsPasswordValid
        {
            get { return _isPasswordValid; }
            set
            {
                SetProperty(ref _isPasswordValid, value);
            }
        }

        private bool _isFormValid;
        public bool IsFormValid
        {
            get { return _isFormValid; }
            set
            {
                SetProperty(ref _isFormValid, value);
            }
        }

        public RestoreWalletPageViewModel(INavigationService navigationService, IDataProvider dataProvider, IDialogService dialogService, IPageDialogService pageDialogService)
            : base(navigationService, dataProvider, dialogService, pageDialogService)
        {
        }

        public DelegateCommand RestoreWalletCommand => new DelegateCommand(RestoreWallet);

        private async void RestoreWallet()
        {
            var login = await DataProvider.RestoreWallet(Username, Password, WalletSeed);
            if (!string.IsNullOrEmpty(login.Token))
            {
                await SecureStorage.SetAsync("token", login.Token);
                await SecureStorage.SetAsync("username", Username);
                await SecureStorage.SetAsync("slatepack_address", login.SlatepackAdddress);
                await SecureStorage.SetAsync("tor_address", login.TorAdddress);

                await NavigationService.NavigateAsync("/SharedTransitionNavigationPage/DashboardCarouselPage", new NavigationParameters { { "wallet", Username } });
            }
        }
    }
}
