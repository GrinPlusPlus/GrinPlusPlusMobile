using Prism;
using Prism.Ioc;
using GrinPlusPlus.ViewModels;
using GrinPlusPlus.Views;
using Xamarin.Essentials.Interfaces;
using Xamarin.Essentials.Implementation;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Threading;
using System.Globalization;
using GrinPlusPlus.Services;
using GrinPlusPlus.Dialogs;
using Plugin.SharedTransitions;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace GrinPlusPlus
{
    public partial class App
    {
        public App() : this(null) { }

        public App(IPlatformInitializer initializer) : base(initializer) { }

        protected override async void OnInitialized()
        {
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InstalledUICulture;

            InitializeComponent();

            await NavigationService.NavigateAsync("/SharedTransitionNavigationPage/LoginPage");
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<IDataProvider, MockDataProvider>();
            containerRegistry.RegisterSingleton<IAppInfo, AppInfoImplementation>();

            containerRegistry.RegisterForNavigation<SharedTransitionNavigationPage>();
            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<DashboardCarouselPage, DashboardCarouselPageViewModel>();
            containerRegistry.RegisterForNavigation<LoginPage, LoginPageViewModel>();
            containerRegistry.RegisterForNavigation<CreateWalletPage, CreateWalletPageViewModel>();
            containerRegistry.RegisterForNavigation<RestoreWalletPage, RestoreWalletPageViewModel>();
            containerRegistry.RegisterForNavigation<WalletPage, WalletPageViewModel>();
            containerRegistry.RegisterForNavigation<TransactionDetailsPage, TransactionDetailsPageViewModel>();
            containerRegistry.RegisterForNavigation<ReceivePage, ReceivePageViewModel>();
            containerRegistry.RegisterForNavigation<SetAmountPage, SetAmountPageViewModel>();
            containerRegistry.RegisterForNavigation<TansactionsHistoryPage, TansactionsHistoryPageViewModel>();
            containerRegistry.RegisterForNavigation<SendingGrinsPage, SendingGrinsPageViewModel>();
            containerRegistry.RegisterForNavigation<EnterAddressMessagePage, EnterAddressMessagePageViewModel>();

            containerRegistry.RegisterDialog<AccountPasswordDialogView, AccountPasswordDialogViewModel>();
            containerRegistry.RegisterDialog<ReceiveTransactionDialogView, ReceiveTransactionDialogViewModel>();
            containerRegistry.RegisterDialog<FinalizeTransactionDialogView, FinalizeTransactionDialogViewModel>();
        }
    }
}
