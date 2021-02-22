﻿using GrinPlusPlus.Api;
using GrinPlusPlus.Models;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using Prism.Services.Dialogs;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace GrinPlusPlus.ViewModels
{
    public class TansactionsHistoryPageViewModel : ViewModelBase
    {
        private ObservableCollection<TransactionGroup> _transactions = new ObservableCollection<TransactionGroup>();
        public ObservableCollection<TransactionGroup> Transactions
        {
            get { return _transactions; }
            set { SetProperty(ref _transactions, value); }
        }

        private Transaction _selectedTransaction;
        public Transaction SelectedTransaction
        {
            get { return _selectedTransaction; }
            set
            {
                SetProperty(ref _selectedTransaction, value);
            }
        }

        public string Token = string.Empty;

        public DelegateCommand OpenTransactionDetailsCommand => new DelegateCommand(OpenTransactionDetails);

        public TansactionsHistoryPageViewModel(INavigationService navigationService, IDataProvider dataProvider, IDialogService dialogService, IPageDialogService pageDialogService)
            : base(navigationService, dataProvider, dialogService, pageDialogService)
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                Token = await SecureStorage.GetAsync("token");

                await LoadWalletHistory();
            });

            Device.StartTimer(TimeSpan.FromSeconds(5), () =>
            {
                if (Settings.IsLoggedIn == false)
                {
                    return false;
                }

                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await LoadWalletHistory();
                });

                return true;
            });
        }

        private async Task LoadWalletHistory()
        {
            try
            {
                var transactionsGroupedByDate = (await DataProvider.GetTransactions(Token,
                                                       new string[] { "COINBASE", "SENT", "RECEIVED", "SENT_CANCELED", "RECEIVED_CANCELED" })
                                                ).GroupBy(x => x.Date.ToString("dddd, dd MMMM yyyy"));
                
                if (Transactions.Count == 0)
                {
                    foreach (var group in transactionsGroupedByDate)
                    {
                        Transactions.Add(new TransactionGroup(group.Key,
                                transactionsGroupedByDate.First(g => g.Key.Equals(group.Key)).ToList()));
                    }

                    return;
                }

                var update = false;

                foreach (IGrouping<string, Transaction> group in transactionsGroupedByDate)
                {
                    var date = Transactions.First(t => t.Name.Equals(group.Key));
                    if (date != null)
                    {
                        if (group.Count() != date.Count())
                        {
                            update = true;
                            break;
                        }
                    }
                    if (!Transactions.Any(t => t.Name.Equals(group.Key)))
                    {
                        update = true;
                        break;
                    }
                }

                if (update)
                {
                    Transactions = new ObservableCollection<TransactionGroup>();

                    foreach (var group in transactionsGroupedByDate)
                    {
                        Transactions.Add(new TransactionGroup(group.Key,
                                transactionsGroupedByDate.First(g => g.Key.Equals(group.Key)).ToList()));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        async void OpenTransactionDetails()
        {
            if (SelectedTransaction is null) return;
            await NavigationService.NavigateAsync("TransactionDetailsPage", new NavigationParameters { { "transaction", SelectedTransaction } });
            SelectedTransaction = null;
        }
    }
}
