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
    public class RestoreWalletPageViewModel : ViewModelBase
    {
        private string _username = "";
        public string Username
        {
            get { return _username.Trim().Replace(" ", ""); }
            set { SetProperty(ref _username, value.Trim()); }
        }

        private string _password = "";
        public string Password
        {
            get { return _password.Trim(); }
            set { SetProperty(ref _password, value); }
        }

        private string _passwordConfirmation = "";
        public string PasswordConfirmation
        {
            get { return _passwordConfirmation.Trim(); }
            set { SetProperty(ref _passwordConfirmation, value); }
        }

        private string _walletSeed = "";
        public string WalletSeed
        {
            get { return _walletSeed; }
            set
            {
                SetProperty(ref _walletSeed, value.Trim());
                IsFormValid = IsUsernameValid && IsPasswordConfirmationValid && IsPasswordValid && value.Trim().Split(' ').Length >= 12;
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

        private bool _isPasswordConfirmationValid;
        public bool IsPasswordConfirmationValid
        {
            get { return _isPasswordConfirmationValid; }
            set
            {
                SetProperty(ref _isPasswordConfirmationValid, value);
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

        private bool _isBusy = false;
        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        private bool _isNodeFullySynched = false;
        public bool IsNodeFullySynched
        {
            get => _isNodeFullySynched;
            set => SetProperty(ref _isNodeFullySynched, value);
        }

        public RestoreWalletPageViewModel(INavigationService navigationService, IDataProvider dataProvider, IDialogService dialogService, IPageDialogService pageDialogService)
            : base(navigationService, dataProvider, dialogService, pageDialogService)
        {
            if(Settings.Node.HeaderHeight == Settings.Node.NetworkHeight)
            {
                if(Settings.Node.HeaderHeight == Settings.Node.Blocks)
                {
                    IsNodeFullySynched = true;
                }
            }
        }

        public DelegateCommand RestoreWalletCommand => new DelegateCommand(RestoreWallet);

        private async void RestoreWallet()
        {
            if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password) || string.IsNullOrEmpty(Password))
            {
                string message = AppResources.ResourceManager.GetString("FillEmptyBoxes");
                await PageDialogService.DisplayAlertAsync("Error", message, "OK");
                return;
            }

            if (Password != PasswordConfirmation)
            {
                string message = AppResources.ResourceManager.GetString("ConfirmPassword");
                await PageDialogService.DisplayAlertAsync("Error", message, "OK");
                return;
            }

            if (!Settings.Node.Status.Equals("Running"))
            {
                string message = AppResources.ResourceManager.GetString("WaitFullySynced");
                await PageDialogService.DisplayAlertAsync("Error", message, "OK");
                return;
            }

            try
            {
                IsBusy = true;

                var login = await DataProvider.RestoreWallet(Username, Password, WalletSeed);
                await SecureStorage.SetAsync("token", login.Token);
                await SecureStorage.SetAsync("username", Username);
                await SecureStorage.SetAsync("slatepack_address", login.SlatepackAdddress);
                await SecureStorage.SetAsync("tor_address", "");

                await NavigationService.NavigateAsync("OpeningWalletPage");
            }
            catch (Exception ex)
            {
                await PageDialogService.DisplayAlertAsync("Error", ex.Message, "OK");

                Debug.WriteLine(ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
