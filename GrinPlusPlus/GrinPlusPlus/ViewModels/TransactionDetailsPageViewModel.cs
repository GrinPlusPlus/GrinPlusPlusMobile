using GrinPlusPlus.Api;
using GrinPlusPlus.Models;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using Prism.Services.Dialogs;
using System;
using System.Diagnostics;
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

        private bool _hasFee = false;
        public bool HasFee
        {
            get { return _hasFee; }
            set { SetProperty(ref _hasFee, value); }
        }

        private bool _canBeReposted = false;
        public bool CanBeReposted
        {
            get { return _canBeReposted; }
            set { SetProperty(ref _canBeReposted, value); }
        }

        public DelegateCommand<object> RepostTransactionCommand => new DelegateCommand<object>(RepostTransaction);
        public DelegateCommand<object> CopyTextCommand => new DelegateCommand<object>(CopyText);

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
                HasFee = SelectedTransaction.Fee > 0;
                CanBeReposted = Helpers.CleanTxType(SelectedTransaction.Status).Equals("sending_finalized");
            }
        }

        async void CopyText(object text)
        {
            await Clipboard.SetTextAsync((string)text);
        }

        async void RepostTransaction(object text)
        {
            string token = await SecureStorage.GetAsync("token");
            try
            {
                await DataProvider.RepostTransaction(token, SelectedTransaction.Id);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            finally
            {
                await NavigationService.GoBackToRootAsync();
            }
        }
    }
}
