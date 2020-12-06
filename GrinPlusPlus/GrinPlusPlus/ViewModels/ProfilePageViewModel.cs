using GrinPlusPlus.Api;
using GrinPlusPlus.Resources;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using Prism.Services.Dialogs;
using System;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace GrinPlusPlus.ViewModels
{
    public class ProfilePageViewModel : ViewModelBase
    {
        private bool _reachable;
        public bool Reachable
        {
            get { return _reachable; }
            set { SetProperty(ref _reachable, value); }
        }

        private string _slatepackAddress = "";
        public string SlatepackAddress
        {
            get { return _slatepackAddress; }
            set { SetProperty(ref _slatepackAddress, value); }
        }

        private string _torAddress = "";
        public string TorAddress
        {
            get { return _torAddress; }
            set { SetProperty(ref _torAddress, value); }
        }

        private string _addressColor = "White";
        public string AddressColor
        {
            get { return _addressColor; }
            set { SetProperty(ref _addressColor, value); }
        }

        public DelegateCommand CopyAddressCommand => new DelegateCommand(CopyAddress);

        private async void CopyAddress()
        {
            await Clipboard.SetTextAsync(SlatepackAddress);
        }

        public DelegateCommand ShareAddressCommand => new DelegateCommand(ShareAddress);

        private async void ShareAddress()
        {
            var share = AppResources.ResourceManager.GetString("ShareAddress");
            var close = AppResources.ResourceManager.GetString("Close");
            var slatepack = AppResources.ResourceManager.GetString("SlatepackAddress");
            var tor = AppResources.ResourceManager.GetString("TorAddress");

            var selectedMethod = await PageDialogService.DisplayActionSheetAsync(share, close, null, slatepack, tor);
            if (selectedMethod == null)
            {
                return;
            }

            if (selectedMethod.Equals(slatepack))
            {
                await Share.RequestAsync(new ShareTextRequest
                {
                    Text = SlatepackAddress,
                    Title = "grin"
                });
            }
            else if (selectedMethod.Equals(tor))
            {
                await Share.RequestAsync(new ShareTextRequest
                {
                    Text = TorAddress,
                    Title = "grin"
                });
            }
        }

        public DelegateCommand OpenBackupWalletPageCommand => new DelegateCommand(OpenBackupWalletPage);

        private async void OpenBackupWalletPage()
        {
            await NavigationService.NavigateAsync("BackupWalletPage", new NavigationParameters { { "username", await SecureStorage.GetAsync("username") } });
        }

        public ProfilePageViewModel(INavigationService navigationService, IDataProvider dataProvider, IDialogService dialogService, IPageDialogService pageDialogService)
            : base(navigationService, dataProvider, dialogService, pageDialogService)
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                SlatepackAddress = await SecureStorage.GetAsync("slatepack_address");
                TorAddress = await SecureStorage.GetAsync("tor_address");
                await UpdateAvailability();
            });
            

            Device.StartTimer(TimeSpan.FromSeconds(10), () =>
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await UpdateAvailability();
                });
                
                return Settings.IsLoggedIn;
            });
        }

        async Task UpdateAvailability()
        {
            if (string.IsNullOrEmpty(TorAddress))
            {
                Reachable = false;
                AddressColor = "Orange";
                return;
            }
            try
            {
                Reachable = await DataProvider.CheckAddressAvailability(TorAddress);
                AddressColor = Reachable ? "Green" : "Orange";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
