using GrinPlusPlus.Api;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using Prism.Services.Dialogs;

namespace GrinPlusPlus.ViewModels
{
    public class ViewModelBase : BindableBase, IInitialize, INavigationAware, IDestructible
    {
        protected INavigationService NavigationService { get; private set; }
        protected IDataProvider DataProvider { get; private set; }
        protected IDialogService DialogService { get; private set; }
        protected IPageDialogService PageDialogService { get; private set; }

        public DelegateCommand OpenSettingsScreenCommand => new DelegateCommand(OpenSettingsScreen);
        public DelegateCommand OpenNodeStatusScreenCommand => new DelegateCommand(OpenNodeStatusScreen);

        private string _title;
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public ViewModelBase(INavigationService navigationService, IDataProvider dataProvider, IDialogService dialogService, IPageDialogService pageDialogService)
        {
            NavigationService = navigationService;
            DataProvider = dataProvider;
            DialogService = dialogService;
            PageDialogService = pageDialogService;
        }

        public virtual void Initialize(INavigationParameters parameters)
        {

        }

        public virtual void OnNavigatedFrom(INavigationParameters parameters)
        {

        }

        public virtual void OnNavigatedTo(INavigationParameters parameters)
        {

        }

        public virtual void Destroy()
        {

        }

        async void OpenSettingsScreen()
        {
            await NavigationService.NavigateAsync("SettingsPage");
        }

        async void OpenNodeStatusScreen()
        {
            await NavigationService.NavigateAsync("StatusPage");
        }
    }
}
