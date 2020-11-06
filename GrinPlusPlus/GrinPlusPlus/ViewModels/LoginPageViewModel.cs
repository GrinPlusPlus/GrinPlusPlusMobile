using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Plugin.Fingerprint;
using System.Threading;
using Plugin.Fingerprint.Abstractions;
using System.Collections.ObjectModel;
using GrinPlusPlus.Models;
using Prism.Services;
using Prism.Services.Dialogs;

namespace GrinPlusPlus.ViewModels
{
    public class LoginPageViewModel : ViewModelBase
    {
        public ObservableCollection<Account> Accounts { get; set; }

        private CancellationTokenSource _cancel;

        private readonly IDialogService _pageDialogService;

        public DelegateCommand<string> AccountNameClickedCommand => new DelegateCommand<string>(AccountNameClicked);

        public LoginPageViewModel(INavigationService navigationService, IDialogService pageDialogService)
            : base(navigationService)
        {
            _pageDialogService = pageDialogService;

            Accounts = new ObservableCollection<Account>();
            foreach (var account in new string[3] { "donations", "personal", "testing" })
            {
                Accounts.Add(new Account() { Name = account});
            }
        }

        async void AccountNameClicked(string userName)
        {
            if (await CrossFingerprint.Current.IsAvailableAsync(true))
            {
                _cancel = new CancellationTokenSource();
                var dialogConfig = new AuthenticationRequestConfiguration("Grin++", $"Authenticate to open {userName.ToUpper()} Wallet")
                {
                    CancelTitle = null,
                    FallbackTitle = null,
                    AllowAlternativeAuthentication = true
                };

                var result = await CrossFingerprint.Current.AuthenticateAsync(dialogConfig, _cancel.Token);

                if (result.Authenticated)
                {
                    await NavigationService.NavigateAsync("/SharedTransitionNavigationPage/DashboardCarouselPage");
                }
            } else
            {
                var parameters = new DialogParameters {{ "username", userName }};
                await _pageDialogService.ShowDialogAsync("AccountPasswordDialogView", parameters);
            }
        }
    }
}
