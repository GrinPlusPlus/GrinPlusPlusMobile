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
        public ObservableCollection<Account> Accounts { get; set; }

        public DelegateCommand<string> AccountNameClickedCommand => new DelegateCommand<string>(AccountNameClicked);

        public LoginPageViewModel(INavigationService navigationService, IDataProvider dataProvider, IDialogService dialogService, IPageDialogService pageDialogService)
            : base(navigationService, dataProvider, dialogService, pageDialogService)
        {
            Accounts = new ObservableCollection<Account>();
            Debug.WriteLine(Battery.EnergySaverStatus);
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                try
                {
                    foreach (var account in await dataProvider.GetAccounts())
                    {
                        Accounts.Add(new Account() { Name = account.Name });
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            });
        }

        async void AccountNameClicked(string userName)
        {
            await DialogService.ShowDialogAsync("AccountPasswordDialogView", new DialogParameters { { "username", userName } });
        }
    }
}
