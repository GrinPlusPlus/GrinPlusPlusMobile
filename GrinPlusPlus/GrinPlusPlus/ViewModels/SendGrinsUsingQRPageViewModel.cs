using GrinPlusPlus.Api;
using GrinPlusPlus.Models;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using Prism.Services.Dialogs;
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

        public DelegateCommand CopySlatepackMessageCommand => new DelegateCommand(CopySlatepackMessage);

        public DelegateCommand ShareSlatepackMessageCommand => new DelegateCommand(ShareSlatepackMessage);

        public DelegateCommand FinalizeTransactionCommand => new DelegateCommand(FinalizeTransaction);

        public DelegateCommand CloseScreenCommand => new DelegateCommand(CloseScreen);

        public SendGrinsUsingQRPageViewModel(INavigationService navigationService, IDataProvider dataProvider, IDialogService dialogService, IPageDialogService pageDialogService)
            : base(navigationService, dataProvider, dialogService, pageDialogService)
        {

        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            if (parameters.ContainsKey("sending_response"))
            {
                SendingResponse = (SendingResponse)parameters["sending_response"];
            }
        }

        private async void CopySlatepackMessage()
        {
            await Clipboard.SetTextAsync(SendingResponse.Slatepack);
        }

        private async void ShareSlatepackMessage()
        {
            await Share.RequestAsync(new ShareTextRequest
            {
                Text = SendingResponse.Slatepack,
                Title = "$grin"
            });
        }

        private async void FinalizeTransaction()
        {
            await NavigationService.NavigateAsync("FinalizeTransactionPage");
        }


        private async void CloseScreen()
        {
            await NavigationService.GoBackToRootAsync();
        }
    }
}
