using GrinPlusPlus.Api;
using GrinPlusPlus.Models;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using Prism.Services.Dialogs;
using System;
using System.IO;
using Xamarin.Essentials;

namespace GrinPlusPlus.ViewModels
{
    public class SendGrinsUsingQRPageViewModel : ViewModelBase
    {
        private SendingResponse _sendingResponse;
        public SendingResponse SendingResponse
        {
            get { return _sendingResponse; }
            set { SetProperty(ref _sendingResponse, value); }
        }

        private double _amount = 0;
        public double Amount
        {
            get { return _amount; }
            set { SetProperty(ref _amount, value); }
        }

        private string[] _inputs;
        public string[] Inputs
        {
            get { return _inputs; }
            set { SetProperty(ref _inputs, value); }
        }

        private string _address = "";
        public string Address
        {
            get { return _address; }
            set { SetProperty(ref _address, value); }
        }

        private string _message = "-";
        public string Message
        {
            get { return _message; }
            set { SetProperty(ref _message, value); }
        }

        private bool _isBusy = true;
        public bool IsBusy
        {
            get { return _isBusy; }
            set { SetProperty(ref _isBusy, value); }
        }

        private string _exceptionMessage;
        public string ExceptionMessage
        {
            get { return _exceptionMessage; }
            set { SetProperty(ref _exceptionMessage, value); }
        }

        public DelegateCommand CopySlatepackMessageCommand => new DelegateCommand(CopySlatepackMessage);

        private async void CopySlatepackMessage()
        {
            await Clipboard.SetTextAsync(SendingResponse.Slatepack);
        }

        public DelegateCommand ShareSlatepackMessageCommand => new DelegateCommand(ShareSlatepackMessage);

        private async void ShareSlatepackMessage()
        {
            await Share.RequestAsync(new ShareTextRequest
            {
                Text = SendingResponse.Slatepack,
                Title = "$grin"
            });
        }

        public DelegateCommand ShareSlatepackMessageAsFileCommand => new DelegateCommand(ShareSlatepackMessageAsFile);

        private async void ShareSlatepackMessageAsFile()
        {
            var file = Path.Combine(FileSystem.CacheDirectory, "message.slate");
            File.WriteAllText(file, SendingResponse.Slatepack);

            await Share.RequestAsync(new ShareFileRequest
            {
                Title = "$grin",
                File = new ShareFile(file)
            });
        }

        public DelegateCommand FinalizeTransactionCommand => new DelegateCommand(FinalizeTransaction);

        private async void FinalizeTransaction()
        {
            await NavigationService.NavigateAsync("FinalizeTransactionPage");
        }

        public DelegateCommand CloseScreenCommand => new DelegateCommand(CloseScreen);

        private async void CloseScreen()
        {
            await NavigationService.GoBackToRootAsync();
        }

        public SendGrinsUsingQRPageViewModel(INavigationService navigationService, IDataProvider dataProvider, IDialogService dialogService, IPageDialogService pageDialogService)
            : base(navigationService, dataProvider, dialogService, pageDialogService)
        {
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            if (parameters.ContainsKey("amount"))
            {
                Amount = (double)parameters["amount"];
            }

            if (parameters.ContainsKey("address"))
            {
                Address = (string)parameters["address"];
            }

            if (parameters.ContainsKey("message"))
            {
                Message = (string)parameters["message"];
            }

            if (parameters.ContainsKey("inputs"))
            {
                Inputs = (string[])parameters["inputs"];
            }

            if (!string.IsNullOrEmpty(Address) && Inputs.Length > 0 && Amount > 0)
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    try
                    {
                        SendingResponse = await DataProvider.SendGrins(
                            await SecureStorage.GetAsync("token"), Address, Amount, Inputs);
                    }
                    catch (Exception ex)
                    {
                        ExceptionMessage = ex.Message;
                    }
                    IsBusy = false;
                });
            }
        }
    }
}
