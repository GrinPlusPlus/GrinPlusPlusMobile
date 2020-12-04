using GrinPlusPlus.Api;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using Prism.Services.Dialogs;
using System;
using System.Diagnostics;
using Xamarin.Essentials;

namespace GrinPlusPlus.ViewModels
{
    public class CreateWalletPageViewModel : ViewModelBase
    {
        private string _exceptionMessage = "";
        public string ExceptionMessage
        {
            get { return _exceptionMessage; }
            set { SetProperty(ref _exceptionMessage, value.Trim()); }
        }

        private string _username = "";
        public string Username
        {
            get { return _username.Trim().Replace(" ", ""); }
            set { SetProperty(ref _username, value.Trim()); }
        }

        private string _password = "";
        public string Password
        {
            get { return _password; }
            set { SetProperty(ref _password, value); }
        }

        private string _passwordConfirmation = "";
        public string PasswordConfirmation
        {
            get { return _passwordConfirmation; }
            set { SetProperty(ref _passwordConfirmation, value); }
        }

        private string _seedLength = "24";
        public string SeedLength
        {
            get { return _seedLength; }
            set { SetProperty(ref _seedLength, value); }
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

        public CreateWalletPageViewModel(INavigationService navigationService, IDataProvider dataProvider, IDialogService dialogService, IPageDialogService pageDialogService)
            : base(navigationService, dataProvider, dialogService, pageDialogService)
        {
        }

        public DelegateCommand CreateWalletCommand => new DelegateCommand(CreateWallet);

        private async void CreateWallet()
        {
            try
            {
                ExceptionMessage = string.Empty;

                IsBusy = true;
                IsIdle = false;

                var wallet = await DataProvider.CreateWallet(Username, Password, int.Parse(SeedLength));

                if (!string.IsNullOrEmpty(wallet.Token))
                {
                    await SecureStorage.SetAsync("token", wallet.Token);
                    await SecureStorage.SetAsync("username", Username);
                    await SecureStorage.SetAsync("slatepack_address", wallet.SlatepackAdddress);

                    await NavigationService.NavigateAsync("OpeningWalletPage");
                }
            }
            catch (Exception ex)
            {
                ExceptionMessage = ex.Message;
            } finally
            {
                IsBusy = false;
                IsIdle = true;
            }
        }
    }
}
