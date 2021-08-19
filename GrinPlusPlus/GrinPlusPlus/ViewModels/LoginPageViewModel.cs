using GrinPlusPlus.Api;
using GrinPlusPlus.Models;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using Prism.Services.Dialogs;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace GrinPlusPlus.ViewModels
{
    public class LoginPageViewModel : ViewModelBase
    {
        private ObservableCollection<Account> _accounts;
        public ObservableCollection<Account> Accounts
        {
            get { return _accounts; }
            set { SetProperty(ref _accounts, value); }
        }

        public DelegateCommand<string> AccountNameClickedCommand => new DelegateCommand<string>(AccountNameClicked);

        public LoginPageViewModel(INavigationService navigationService, IDataProvider dataProvider, IDialogService dialogService, IPageDialogService pageDialogService)
            : base(navigationService, dataProvider, dialogService, pageDialogService)
        {
            Settings.IsLoggedIn = false;

            MainThread.BeginInvokeOnMainThread(async () =>
            {
                try
                {
                    Accounts = new ObservableCollection<Account>((await DataProvider.GetAccounts()).ToArray());
                    // Collect all generations of memory.
                    GC.Collect();
                }

                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            });
        }

        async void AccountNameClicked(string username)
        {
            await NavigationService.NavigateAsync("WalletLoginPage", new NavigationParameters { { "username", username } });
        }
    }
}
