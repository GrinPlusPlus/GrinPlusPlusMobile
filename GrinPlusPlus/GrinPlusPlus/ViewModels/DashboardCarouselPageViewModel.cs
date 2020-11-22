using GrinPlusPlus.Api;
using Prism.Navigation;
using Prism.Services;
using Prism.Services.Dialogs;

namespace GrinPlusPlus.ViewModels
{
    public class DashboardCarouselPageViewModel : ViewModelBase
    {
        private string _wallet = "";
        public string Wallet
        {
            get { return _wallet; }
            set { SetProperty(ref _wallet, value); }
        }

        public DashboardCarouselPageViewModel(INavigationService navigationService, IDataProvider dataProvider, IDialogService dialogService, IPageDialogService pageDialogService)
            : base(navigationService, dataProvider, dialogService, pageDialogService)
        {
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            if (parameters.ContainsKey("wallet"))
            {
                Wallet = (string)parameters["wallet"];
            }
        }
    }
}
