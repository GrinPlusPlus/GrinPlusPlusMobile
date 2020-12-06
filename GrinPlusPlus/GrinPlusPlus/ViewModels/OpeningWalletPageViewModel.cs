using GrinPlusPlus.Api;
using Prism.Navigation;
using Prism.Services;
using Prism.Services.Dialogs;
using System;
using System.Threading;
using Xamarin.Essentials;

namespace GrinPlusPlus.ViewModels
{
    public class OpeningWalletPageViewModel : ViewModelBase
    {
        private string _exceptionMessage;
        public string ExceptionMessage
        {
            get { return _exceptionMessage; }
            set { SetProperty(ref _exceptionMessage, value); }
        }


        public OpeningWalletPageViewModel(INavigationService navigationService, IDataProvider dataProvider, IDialogService dialogService, IPageDialogService pageDialogService)
            : base(navigationService, dataProvider, dialogService, pageDialogService)
        {
            
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                try
                {
                    var balance = await DataProvider.GetWalletBalance(await SecureStorage.GetAsync("token"));

                    Preferences.Set("balance_spendable", balance.Spendable);
                    Preferences.Set("balance_locked", balance.Locked);
                    Preferences.Set("balance_immature", balance.Immature);
                    Preferences.Set("balance_unconfirmed", balance.Unconfirmed);
                    Preferences.Set("balance_total", balance.Total);

                    await NavigationService.NavigateAsync("/SharedTransitionNavigationPage/DashboardCarouselPage", new NavigationParameters { { "wallet", await SecureStorage.GetAsync("username") } });
                }
                catch (Exception ex)
                {
                    ExceptionMessage = ex.Message;
                    Thread.Sleep(3000);
                    await NavigationService.GoBackToRootAsync();
                }
            });
        }
    }
}
