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
using System.Threading;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace GrinPlusPlus.ViewModels
{
    public class LoginPageViewModel : ViewModelBase
    {
        int failCounter = 0;

        private ObservableCollection<Account> _accounts = new ObservableCollection<Account>();
        public ObservableCollection<Account> Accounts
        {
            get { return _accounts; }
            set { SetProperty(ref _accounts, value); }
        }

        public DelegateCommand<string> AccountNameClickedCommand => new DelegateCommand<string>(AccountNameClicked);

        public LoginPageViewModel(INavigationService navigationService, IDataProvider dataProvider, IDialogService dialogService, IPageDialogService pageDialogService)
            : base(navigationService, dataProvider, dialogService, pageDialogService)
        {
            Device.StartTimer(TimeSpan.FromSeconds(0), () =>
            {
                if (Settings.IsLoggedIn == true || failCounter.Equals(100))
                {
                    return false;
                }

                ListWallets();

                return true;
            });
        }

        private void ListWallets()
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                try
                {
                    var accounts = await DataProvider.GetAccounts();
                    if(Accounts.ToList().Count != accounts.Count)
                    {
                       Accounts = new ObservableCollection<Account>(accounts.ToArray());
                    }

                    failCounter = 0;
                }

                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    failCounter += 1;
                }
            });
        }

        async void AccountNameClicked(string username)
        {
            await NavigationService.NavigateAsync("WalletLoginPage", new NavigationParameters { { "username", username } });
        }
    }
}
