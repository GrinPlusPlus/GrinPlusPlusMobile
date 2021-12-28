using GrinPlusPlus.Api;
using GrinPlusPlus.Models;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using Prism.Services.Dialogs;
using Xamarin.Essentials;

namespace GrinPlusPlus.ViewModels
{
    public class UnfinalizedTransactionDetailsPageViewModel : ViewModelBase
    {
        private Transaction _transaction;
        public Transaction SelectedTransaction
        {
            get { return _transaction; }
            set { SetProperty(ref _transaction, value); }
        }

        public DelegateCommand CopySlatepackMessageCommand => new DelegateCommand(CopySlatepackMessage);

        public DelegateCommand ShareSlatepackMessageCommand => new DelegateCommand(ShareSlatepackMessage);

        private async void CopySlatepackMessage()
        {
            await Clipboard.SetTextAsync(SelectedTransaction.Message);
        }

        private async void ShareSlatepackMessage()
        {
            await Share.RequestAsync(new ShareTextRequest
            {
                Text = SelectedTransaction.Message,
                Title = "$grin"
            });
        }


        public UnfinalizedTransactionDetailsPageViewModel(INavigationService navigationService, IDataProvider dataProvider, IDialogService dialogService, IPageDialogService pageDialogService)
            : base(navigationService, dataProvider, dialogService, pageDialogService)
        {
            
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            if (parameters.ContainsKey("transaction"))
            {
                SelectedTransaction = (Transaction)parameters["transaction"];
            }
        }
    }
}
