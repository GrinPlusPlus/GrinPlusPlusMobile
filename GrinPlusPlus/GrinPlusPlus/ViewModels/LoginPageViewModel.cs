using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GrinPlusPlus.ViewModels
{
    public class LoginPageViewModel : ViewModelBase
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

        public DelegateCommand OpenWalletCommand => new DelegateCommand(OpenWallet);
        public DelegateCommand CreateWalletCommand => new DelegateCommand(CreateWallet);
        public DelegateCommand RestoreWalletCommand => new DelegateCommand(RestoreWallet);

        private void OpenWallet()
        {
            Title = DateTime.Now.ToString();
        }

        private void CreateWallet()
        {
            Title = DateTime.Now.ToString();
        }

        private void RestoreWallet()
        {
            Title = DateTime.Now.ToString();
        }

        public LoginPageViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            Title = "Grin++";
        }
    }
}
