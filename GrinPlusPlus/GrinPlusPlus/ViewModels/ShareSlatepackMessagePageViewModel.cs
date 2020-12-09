using Prism.Commands;
using GrinPlusPlus.Api;
using Prism.Navigation;
using Prism.Services;
using Prism.Services.Dialogs;
using System.IO;
using Xamarin.Essentials;
using GrinPlusPlus.Models;

namespace GrinPlusPlus.ViewModels
{
    public class ShareSlatepackMessagePageViewModel : ViewModelBase
    {
        private ReceivingResponse _receivingResponse = new ReceivingResponse()
        {
            Slatepack = string.Empty
        };
        public ReceivingResponse ReceivingResponse
        {
            get { return _receivingResponse; }
            set { SetProperty(ref _receivingResponse, value); }
        }

        public DelegateCommand CopySlatepackMessageCommand => new DelegateCommand(CopySlatepackMessage);

        private async void CopySlatepackMessage()
        {
            await Clipboard.SetTextAsync(ReceivingResponse.Slatepack);
        }

        public DelegateCommand ShareSlatepackMessageCommand => new DelegateCommand(ShareSlatepackMessage);

        private async void ShareSlatepackMessage()
        {
            await Share.RequestAsync(new ShareTextRequest
            {
                Text = ReceivingResponse.Slatepack,
                Title = "$grin"
            });
        }

        public DelegateCommand CloseScreenCommand => new DelegateCommand(CloseScreen);

        private async void CloseScreen()
        {
            await NavigationService.GoBackToRootAsync();
        }

        public ShareSlatepackMessagePageViewModel(INavigationService navigationService, IDataProvider dataProvider, IDialogService dialogService, IPageDialogService pageDialogService)
            : base(navigationService, dataProvider, dialogService, pageDialogService)
        {
            
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            if (parameters.ContainsKey("receiving_response"))
            {
                ReceivingResponse = (ReceivingResponse)parameters["receiving_response"];
            }
        }
    }
}
