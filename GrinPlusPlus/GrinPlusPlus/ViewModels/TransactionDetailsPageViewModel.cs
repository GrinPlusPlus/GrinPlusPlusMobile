using GrinPlusPlus.Api;
using GrinPlusPlus.Models;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using Prism.Services.Dialogs;
using Xamarin.Essentials;

namespace GrinPlusPlus.ViewModels
{
    public class TransactionDetailsPageViewModel : ViewModelBase
    {
        private Transaction _transaction;
        public Transaction SelectedTransaction
        {
            get { return _transaction; }
            set { SetProperty(ref _transaction, value); }
        }

        public DelegateCommand<object> CopyTextCommand => new DelegateCommand<object>(CopyText);

        private async void CopyText(object text)
        {
            await Clipboard.SetTextAsync((string)text);
        }


        public TransactionDetailsPageViewModel(INavigationService navigationService,
                                              IDataProvider dataProvider, IDialogService dialogService,
                                              IPageDialogService pageDialogService)
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
