using System;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services.Dialogs;

namespace GrinPlusPlus.ViewModels
{
    public class AccountPasswordDialogViewModel : ViewModelBase, IDialogAware
    {
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

        public AccountPasswordDialogViewModel(INavigationService navigationService) : base(navigationService)
        {
            CloseCommand = new DelegateCommand(async() => {
                RequestClose(null); 
                await NavigationService.NavigateAsync("/NavigationPage/DashboardCarouselPage"); 
            });
        }

        public event Action<IDialogParameters> RequestClose;

        public bool CanCloseDialog() => true;

        public void OnDialogClosed()
        {

        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            if(parameters.ContainsKey("username"))
            {
                Username = parameters.GetValue<string>("username");
            }
        }
    }
}
