using GrinPlusPlus.Api;
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
            await Share.RequestAsync(new ShareTextRequest
            {
                Text = SlatepackAddress,
                Title = "grin"
            });
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
            });


            Device.StartTimer(TimeSpan.FromSeconds(10), () =>
            {
                if(Settings.IsLoggedIn == false)
                {
                    return false;
                }

                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await UpdateAvailability();
                });

                return true;
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
