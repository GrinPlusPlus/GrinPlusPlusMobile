using GrinPlusPlus.Api;
using GrinPlusPlus.ViewModels;
using GrinPlusPlus.Views;
using Plugin.SharedTransitions;
using Prism;
using Prism.Common;
using Prism.Ioc;
using System.Globalization;
using System.Threading;
using Xamarin.Essentials.Implementation;
using Xamarin.Essentials.Interfaces;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

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

            string destination = "/SharedTransitionNavigationPage/InitPage";

            await NavigationService.NavigateAsync(destination);
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<IDataProvider, GrinPPLocalService>();

            containerRegistry.RegisterSingleton<IAppInfo, AppInfoImplementation>();

            containerRegistry.RegisterForNavigation<SharedTransitionNavigationPage>();
            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<LoginPage, LoginPageViewModel>();
            containerRegistry.RegisterForNavigation<CreateWalletPage, CreateWalletPageViewModel>();
            containerRegistry.RegisterForNavigation<RestoreWalletPage, RestoreWalletPageViewModel>();
            containerRegistry.RegisterForNavigation<WalletPage, WalletPageViewModel>();
            containerRegistry.RegisterForNavigation<TransactionDetailsPage, TransactionDetailsPageViewModel>();
            containerRegistry.RegisterForNavigation<ProfilePage, ProfilePageViewModel>();
            containerRegistry.RegisterForNavigation<SetAmountPage, SetAmountPageViewModel>();
            containerRegistry.RegisterForNavigation<SendingGrinsPage, SendingGrinsPageViewModel>();
            containerRegistry.RegisterForNavigation<EnterAddressMessagePage, EnterAddressMessagePageViewModel>();
            containerRegistry.RegisterForNavigation<QRScannerPage, QRScannerPageViewModel>();
            containerRegistry.RegisterForNavigation<SendGrinsUsingTorPage, SendGrinsUsingTorPageViewModel>();
            containerRegistry.RegisterForNavigation<SendGrinsUsingQRPage, SendGrinsUsingQRPageViewModel>();
            containerRegistry.RegisterForNavigation<FinalizeTransactionPage, FinalizeTransactionPageViewModel>();
            containerRegistry.RegisterForNavigation<ReceiveTransactionPage, ReceiveTransactionPageViewModel>();
            containerRegistry.RegisterForNavigation<StatusPage, StatusPageViewModel>();
            containerRegistry.RegisterForNavigation<ShareSlatepackMessagePage, ShareSlatepackMessagePageViewModel>();
            containerRegistry.RegisterForNavigation<WalletLoginPage, WalletLoginPageViewModel>();
            containerRegistry.RegisterForNavigation<InitPage, InitPageViewModel>();
            containerRegistry.RegisterForNavigation<OpeningWalletPage, OpeningWalletPageViewModel>();
            containerRegistry.RegisterForNavigation<BackupWalletPage, BackupWalletPageViewModel>();
            containerRegistry.RegisterForNavigation<SettingsPage, SettingsPageViewModel>();
            containerRegistry.RegisterForNavigation<ErrorPage, ErrorPageViewModel>();
            containerRegistry.RegisterForNavigation<ConfirmWalletSeedPage, ConfirmWalletSeedPageViewModel>();
            containerRegistry.RegisterForNavigation<WalletSeedPage, WalletSeedPageViewModel>();
            containerRegistry.RegisterForNavigation<WalletSeedBackupPage, WalletSeedBackupPageViewModel>();
        }

        protected override void OnSleep()
        {
            base.OnSleep();
            Settings.CurrentPage = PageUtilities.GetCurrentPage(Application.Current.MainPage).ToString().Split('.')[2];
        }

        protected override void OnResume()
        {
            base.OnResume();
        }
    }
}
