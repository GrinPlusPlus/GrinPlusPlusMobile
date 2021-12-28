using GrinPlusPlus.Api;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using Prism.Services.Dialogs;
using System;
using System.Diagnostics;
using Xamarin.Essentials;


namespace GrinPlusPlus.ViewModels
{
    public class FinalizeTransactionPageViewModel : ViewModelBase
    {
        private string _slatepackMessage;
        public string SlatepackMessage
        {
            get { return _slatepackMessage; }
            set { SetProperty(ref _slatepackMessage, value); }
        }

        public DelegateCommand ScanQRCodeCommand => new DelegateCommand(ScanQRCode);

        public DelegateCommand PasteFromClipboardCommand => new DelegateCommand(PasteFromClipboard);

        public DelegateCommand FinalizeTransactionCommand => new DelegateCommand(FinalizeTransaction);

        public FinalizeTransactionPageViewModel(INavigationService navigationService, IDataProvider dataProvider, 
                                                IDialogService dialogService, IPageDialogService pageDialogService)
            : base(navigationService, dataProvider, dialogService, pageDialogService)
        {

        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            var navigationMode = parameters.GetNavigationMode();

            switch (navigationMode)
            {
                case Prism.Navigation.NavigationMode.Back:
                    if (parameters.ContainsKey("qr_scanner_result"))
                    {
                        SlatepackMessage = (string)parameters["qr_scanner_result"];
                    }
                    break;
            }
        }

        private async void ScanQRCode()
        {
            await NavigationService.NavigateAsync(name: "QRScannerPage", parameters: null,
                                                  useModalNavigation: true, animated: true);
        }

        private async void PasteFromClipboard()
        {
            SlatepackMessage = await Clipboard.GetTextAsync();
        }

        private async void FinalizeTransaction()
        {
            try
            {
                var token = await SecureStorage.GetAsync("token");
                var finalized = await DataProvider.FinalizeTransaction(token, SlatepackMessage).ConfigureAwait(false);

                await NavigationService.GoBackToRootAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }
}
