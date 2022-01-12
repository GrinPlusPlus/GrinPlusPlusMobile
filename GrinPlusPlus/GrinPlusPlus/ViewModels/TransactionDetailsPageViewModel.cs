using GrinPlusPlus.Api;
using GrinPlusPlus.Models;
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

        private bool _canBeCanceled = false;
        public bool CanBeCanceled
        {
            get { return _canBeCanceled; }
            set { SetProperty(ref _canBeCanceled, value); }
        }

        public DelegateCommand<object> RepostTransactionCommand => new DelegateCommand<object>(RepostTransaction);
        public DelegateCommand<object> CancelTransactionCommand => new DelegateCommand<object>(CancelTransaction);

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
                CanBeCanceled = Helpers.CleanTxType(SelectedTransaction.Status).Equals("receiving_unconfirmed");
            }
        }

        async void CopyText(object text)
        {
            string commitment = (string)text;
            await Clipboard.SetTextAsync(commitment);

            if (SelectedTransaction.Status.ToLower().Equals("canceled")) return;

            var yes = AppResources.ResourceManager.GetString("Yes");
            var no = AppResources.ResourceManager.GetString("No");
            var confirm = AppResources.ResourceManager.GetString("Confirm");
            var areyousure = AppResources.ResourceManager.GetString("AreYouSure");

            var confirmation = await PageDialogService.DisplayAlertAsync(confirm, areyousure, yes, no);
            if (!confirmation)
            {
                return;
            }

            string uri = $"{Settings.GrinExplorerURL}/output/{commitment}"; 
            try
            {
                await Browser.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);
            }
            catch (Exception ex)
            {
                // An unexpected error occured. No browser may be installed on the device.
                Debug.WriteLine(ex.Message);
            }
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

        async void CancelTransaction(object text)
        {
            string token = await SecureStorage.GetAsync("token");
            try
            {
                await DataProvider.CancelTransaction(token, SelectedTransaction.Id);
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
