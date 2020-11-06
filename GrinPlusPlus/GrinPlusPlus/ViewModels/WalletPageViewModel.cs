using GrinPlusPlus.Models;
using GrinPlusPlus.Services;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using Xamarin.Essentials;


namespace GrinPlusPlus.ViewModels
{
    public class WalletPageViewModel : ViewModelBase
    {
        private readonly IDialogService _pageDialogService;

        private readonly IPageDialogService _dialogService;

        public ObservableCollection<Transaction> InProgressTransactions { get; set; }

        private string _wallet;
        public string Wallet
        {
            get { return _wallet; }
            set { SetProperty(ref _wallet, value); }
        }

        private decimal _spendable = 330052.000000959m;
        public decimal Spendable
        {
            get { return Math.Truncate(_spendable); }
            set { SetProperty(ref _spendable, value); }
        }
        public string SpendableDecimals
        {
            get { return _spendable.ToString("F9").Replace(Math.Truncate(_spendable).ToString(),"").Replace(".","").TrimEnd(new Char[] { '0' }); }
        }

        private Transaction _selectedTransaction;
        public Transaction SelectedTransaction
        {
            get { return _selectedTransaction; }
            set {
                  SetProperty(ref _selectedTransaction, value);
             }
        }

        public DelegateCommand OpenTransactionDetailsCommand => new DelegateCommand(OpenTransactionDetails);

        public DelegateCommand<object> CancelTransactionClickedCommand { get; private set; }

        public DelegateCommand ReceiveButtonClickedCommand => new DelegateCommand(ReceiveButtonClicked);

        public DelegateCommand FinalizeTransactionClickedCommand => new DelegateCommand(FinalizeTransactionClicked);


        public WalletPageViewModel(INavigationService navigationService, IDataProvider dataProvider, IDialogService pageDialogService, IPageDialogService dialogService)
            : base(navigationService)
        {
            _pageDialogService = pageDialogService;
            _dialogService = dialogService;

            Wallet = "donations";
            InProgressTransactions = new ObservableCollection<Transaction>();
            foreach (var transaction in dataProvider.GetAllData())
            {
                if (new string[] { "receiving", "sending" }.Contains(transaction.Status.Trim().ToLower()))
                {
                    InProgressTransactions.Add(transaction);
                }
            }

            CancelTransactionClickedCommand = new DelegateCommand<object>(CancelTransactionClicked);
        }

        async void ReceiveButtonClicked()
        {
            await _pageDialogService.ShowDialogAsync("ReceiveTransactionDialogView");
        }


        async void FinalizeTransactionClicked()
        {
            await _pageDialogService.ShowDialogAsync("FinalizeTransactionDialogView");
        }

        async void CancelTransactionClicked(object id)
        {
            var answer = await _dialogService.DisplayAlertAsync("Cancel Transaction", "Are you sure you want to cancel this Transaction?", "Yes", "No");
            Debug.WriteLine("Answer: " + answer); 
        }

        async void OpenTransactionDetails()
        {
            if (SelectedTransaction is null) return;
            await NavigationService.NavigateAsync("TransactionDetailsPage", new NavigationParameters { { "transaction", SelectedTransaction } });
            SelectedTransaction = null;
        }
    }
}
