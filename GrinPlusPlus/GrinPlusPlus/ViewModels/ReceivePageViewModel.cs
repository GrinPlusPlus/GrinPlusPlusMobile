using GrinPlusPlus.Api;
using GrinPlusPlus.Resources;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using Prism.Services.Dialogs;
using System;
using System.Diagnostics;
using Xamarin.Essentials;

namespace GrinPlusPlus.ViewModels
{
    public class ReceivePageViewModel : ViewModelBase
    {
        private string _slatepackAddress;
        public string SlatepackAddress
        {
            get { return _slatepackAddress; }
            set { SetProperty(ref _slatepackAddress, value); }
        }

        private string _torAddress;
        public string TorAddress
        {
            get { return _torAddress; }
            set { SetProperty(ref _torAddress, value); }
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

        public ReceivePageViewModel(INavigationService navigationService, IDataProvider dataProvider, IDialogService dialogService, IPageDialogService pageDialogService)
            : base(navigationService, dataProvider, dialogService, pageDialogService)
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                try
                {
                    SlatepackAddress = await SecureStorage.GetAsync("slatepack_address");
                    TorAddress = await SecureStorage.GetAsync("tor_address");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            });
        }
    }
}
