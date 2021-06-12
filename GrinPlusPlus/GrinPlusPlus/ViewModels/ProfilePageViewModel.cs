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
        private string _availability = string.Empty;
        public string Availability
        {
            get { return _availability; }
            set { SetProperty(ref _availability, value); }
        }

        private bool _reachable = Settings.Reachable;
        public bool Reachable
        {
            get { return _reachable; }
            set { SetProperty(ref _reachable, value); }
        }

        private string _slatepackAddress = string.Empty;
        public string SlatepackAddress
        {
            get { return _slatepackAddress; }
            set { SetProperty(ref _slatepackAddress, value); }
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

        public ProfilePageViewModel(INavigationService navigationService, IDataProvider dataProvider, IDialogService dialogService, IPageDialogService pageDialogService)
            : base(navigationService, dataProvider, dialogService, pageDialogService)
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                SlatepackAddress = await SecureStorage.GetAsync("slatepack_address");
            });
        }
    }
}
