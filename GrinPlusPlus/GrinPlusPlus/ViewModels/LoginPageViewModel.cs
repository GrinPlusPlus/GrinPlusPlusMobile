using GrinPlusPlus.Api;
using GrinPlusPlus.Models;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using Prism.Services.Dialogs;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Xamarin.Essentials;

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

        }

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            try
            {
                Accounts = new ObservableCollection<Account>((await DataProvider.GetAccounts().ConfigureAwait(false)).ToArray());
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);

                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await PageDialogService.DisplayAlertAsync("Error", ex.Message, "OK");
                });
            }
        }
        
        async void AccountNameClicked(string username)
        {
            await NavigationService.NavigateAsync("WalletLoginPage", new NavigationParameters { { "username", username } });
        }

    }
}
