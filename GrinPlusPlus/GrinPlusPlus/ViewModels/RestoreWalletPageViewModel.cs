using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GrinPlusPlus.ViewModels
{
    public class RestoreWalletPageViewModel : ViewModelBase
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

        private string _walletSeed = "";
        public string WalletSeed
        {
            get { return _walletSeed; }
            set { SetProperty(ref _walletSeed, value); }
        }

        public RestoreWalletPageViewModel(INavigationService navigationService)
            : base(navigationService)
        {
        }

        public DelegateCommand RestoreWalletCommand => new DelegateCommand(RestoreWallet);

        private async void RestoreWallet()
        {
            await NavigationService.NavigateAsync("/SharedTransitionNavigationPage/DashboardCarouselPage");
        }
    }
}
