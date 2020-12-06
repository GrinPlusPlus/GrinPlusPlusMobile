using GrinPlusPlus.Api;
using GrinPlusPlus.Models;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using Prism.Services.Dialogs;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace GrinPlusPlus.ViewModels
{
    public class LoginPageViewModel : ViewModelBase
    {
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
            Device.StartTimer(TimeSpan.FromSeconds(3), () =>
            {
                if (Settings.IsLoggedIn)
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
                    if (accounts.Count == 0)
                    {
                        return;
                    }
                    else
                    {
                        if (Accounts.Count == 0)
                        {
                            Accounts = new ObservableCollection<Account>(accounts.ToArray());
                        }
                        else
                        {
                            var update = false;

                            foreach (var account in Accounts)
                            {
                                if (accounts.Any(a => a.Name.Equals(account.Name)))
                                {
                                    continue;
                                }
                                update = true;
                            }

                            foreach (var account in accounts)
                            {
                                if (!Accounts.Any(a => a.Name.Equals(account.Name)))
                                {
                                    update = true;
                                }
                            }

                            if (update)
                            {
                                Accounts = new ObservableCollection<Account>(accounts.ToArray());
                            }
                        }
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            });
        }

        async void AccountNameClicked(string username)
        {
            await NavigationService.NavigateAsync("WalletLoginPage", new NavigationParameters { { "username", username } });
        }
    }
}
