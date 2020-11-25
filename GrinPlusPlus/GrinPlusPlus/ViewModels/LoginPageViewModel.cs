using GrinPlusPlus.Api;
using GrinPlusPlus.Models;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using Prism.Services.Dialogs;
using System;
using System.Linq;
using System.Collections.ObjectModel;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace GrinPlusPlus.ViewModels
{
    public class LoginPageViewModel : ViewModelBase
    {
        private string _syncStatus = "";
        public string SyncStatus
        {
            get { return _syncStatus.ToUpper(); }
            set { SetProperty(ref _syncStatus, value); }
        }

        public ObservableCollection<Account> Accounts { get; set; }

        public DelegateCommand<string> AccountNameClickedCommand => new DelegateCommand<string>(AccountNameClicked);

        public LoginPageViewModel(INavigationService navigationService, IDataProvider dataProvider, IDialogService dialogService, IPageDialogService pageDialogService)
            : base(navigationService, dataProvider, dialogService, pageDialogService)
        {
            Accounts = new ObservableCollection<Account>();

            Device.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    try
                    {
                        foreach (var account in await DataProvider.GetAccounts())
                        {
                            if (!Accounts.Any(a => a.Name.Equals(account.Name))) {
                                Accounts.Add(new Account() { Name = account.Name });
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                });

                return !Preferences.Get("loggedIn", false);
            });
        }

        async void AccountNameClicked(string username)
        {
            await NavigationService.NavigateAsync("WalletLoginPage", new NavigationParameters { { "username", username } });
        }
    }
}
