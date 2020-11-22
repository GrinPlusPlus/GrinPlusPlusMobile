using GrinPlusPlus.Api;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using Prism.Services.Dialogs;
using System;
using System.Diagnostics;
using System.IO;
using Xamarin.Essentials;


namespace GrinPlusPlus.ViewModels
{
    public class ReceiveTransactionPageViewModel : ViewModelBase
    {
        private string _slatepackMessage;
        public string SlatepackMessage
        {
            get { return _slatepackMessage; }
            set { SetProperty(ref _slatepackMessage, value); }
        }

        public DelegateCommand ScanQRCodeCommand => new DelegateCommand(ScanQRCode);

        private async void ScanQRCode()
        {
            await NavigationService.NavigateAsync(name: "QRScannerPage", parameters: null, useModalNavigation: true, animated: true);
        }

        public DelegateCommand PasteFromClipboardCommand => new DelegateCommand(PasteFromClipboard);

        private async void PasteFromClipboard()
        {
            SlatepackMessage = await Clipboard.GetTextAsync();
        }

        public DelegateCommand LoadFromFileCommand => new DelegateCommand(LoadFromFile);

        private void LoadFromFile()
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                try
                {
                    var result = await FilePicker.PickAsync();
                    if (result != null)
                    {
                        if (result.FileName.EndsWith("slate", StringComparison.OrdinalIgnoreCase))
                        {
                            var stream = await result.OpenReadAsync();
                            StreamReader reader = new StreamReader(stream);
                            SlatepackMessage = reader.ReadToEnd();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            });
        }

        public DelegateCommand CancelCommand => new DelegateCommand(Cancel);

        async void Cancel()
        {
            await NavigationService.GoBackToRootAsync();
        }

        public DelegateCommand ReceiveTransactionCommand => new DelegateCommand(ReceiveTransaction);

        async void ReceiveTransaction()
        {
            try
            {
                await DataProvider.ReceiveTransaction(await SecureStorage.GetAsync("token"), SlatepackMessage);
                await NavigationService.GoBackToRootAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        public ReceiveTransactionPageViewModel(INavigationService navigationService, IDataProvider dataProvider, IDialogService dialogService, IPageDialogService pageDialogService)
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
    }
}
