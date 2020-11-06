using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GrinPlusPlus.ViewModels
{
    public class CreateWalletPageViewModel : ViewModelBase
    {
        private string _username;
        public string Username
        {
            get { return _username; }
            set { SetProperty(ref _username, value); }
        }

        private string _password;
        public string Password
        {
            get { return _password; }
            set { SetProperty(ref _password, value); }
        }

        private string _passwordConfirmation;
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

        public DelegateCommand CreateWalletCommand => new DelegateCommand(CreateWallet);

        private async void CreateWallet()
        {
            await NavigationService.NavigateAsync("/SharedTransitionNavigationPage/DashboardCarouselPage");
        }

        public CreateWalletPageViewModel(INavigationService navigationService)
            : base(navigationService)
        {
        }
    }
}
