using GrinPlusPlus.Api;
using Prism.Navigation;
using Prism.Services;
using Prism.Services.Dialogs;
using Xamarin.Essentials;

namespace GrinPlusPlus.ViewModels
{
    public class InitPageViewModel : ViewModelBase
    {
        public InitPageViewModel(INavigationService navigationService, IDataProvider dataProvider, IDialogService dialogService, IPageDialogService pageDialogService)
            : base(navigationService, dataProvider, dialogService, pageDialogService)
        {
        }

    }
}
