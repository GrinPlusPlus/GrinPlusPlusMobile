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
            Slatepack = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aliquam sit amet odio sodales, faucibus velit et, vulputate lectus. Nam at sem porta, tempus nunc quis, viverra lorem. Phasellus tincidunt tempus odio, a interdum ipsum. Mauris mollis sed quam sit amet interdum. Ut sollicitudin posuere tortor ac molestie. Integer malesuada, enim et gravida feugiat, tellus dolor vulputate mi, non sodales enim massa ac quam. Mauris tincidunt, justo ac interdum sodales, nunc nisl porttitor odio, a accumsan lectus risus a erat. Vivamus sed feugiat odio. Nunc suscipit metus in velit condimentum, ac auctor eros venenatis. Nullam venenatis massa in tempor finibus. Interdum et malesuada fames ac ante ipsum primis in faucibus. Pellentesque tortor metus, vulputate in fringilla ac, convallis eget purus. Pellentesque tempus magna tincidunt odio convallis, et venenatis dui ultrices. Cras a quam lorem. Fusce non egestas neque. Proin ullamcorper tincidunt ipsum a auctor. Sed malesuada elit enim, sit amet vehicula urna commodo id. Morbi sapien justo, tempus vitae justo quis, laoreet suscipit quam. Pellentesque mollis nisi diam, eget pretium nunc dictum nec. Nulla vestibulum velit sed odio dapibus eleifend. Nulla sit amet viverra massa. Duis sit amet gravida ligula. Fusce condimentum hendrerit magna, vel commodo enim convallis at. Etiam venenatis lacinia ligula, vitae ultricies nunc iaculis et. Donec urna magna, bibendum sed velit pulvinar, imperdiet tincidunt ligula."
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
