using GrinPlusPlus.Api;
using GrinPlusPlus.Models;
using GrinPlusPlus.Resources;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace GrinPlusPlus.ViewModels
{
    public class WalletPageViewModel : ViewModelBase
    {
        private List<Transaction> AllTransactions = new List<Transaction>();

        private ObservableCollection<Transaction> _unfinalizedTransactions = new ObservableCollection<Transaction>();
        public ObservableCollection<Transaction> UnfinalizedTransactions
        {
            get
            {
                return _unfinalizedTransactions;
            }
            set
            {
                SetProperty(ref _unfinalizedTransactions, value);
            }
        }

        private ObservableCollection<Transaction> _filteredTransactionHistory = new ObservableCollection<Transaction>();
        public ObservableCollection<Transaction> FilteredTransactionHistory
        {
            get
            {
                return _filteredTransactionHistory;
            }
            set
            {
                SetProperty(ref _filteredTransactionHistory, value);
            }
        }

        private Balance _balance;

        private int _currentSelectedFilterIndex = 1;
        public int CurrentSelectedFilterIndex
        {
            get
            {
                return _currentSelectedFilterIndex;
            }
            set
            {
                if (value == _currentSelectedFilterIndex) return;

                FilterTransactions(value);

                SetProperty(ref _currentSelectedFilterIndex, value);
            }
        }

        private void FilterTransactions(int value)
        {
            switch (value)
            {
                case 1:
                    var pending = AllTransactions.Where(x => new List<string> { "sending", "receiving" }.Any(y => x.Status.ToLower().Contains(y))).ToList();
                    LoadTransactionHistory(pending);
                    break;
                case 2:
                    var received = AllTransactions.Where(x => new List<string> { "received" }.Any(y => y.Equals(x.Status.ToLower()))).ToList();
                    LoadTransactionHistory(received);
                    break;
                case 3:
                    var sent = AllTransactions.Where(x => new List<string> { "sent" }.Any(y => y.Equals(x.Status.ToLower()))).ToList();
                    LoadTransactionHistory(sent);
                    break;
                case 4:
                    var canceled = AllTransactions.Where(x => new List<string> { "canceled" }.Any(y => y.Equals(x.Status.ToLower()))).ToList();
                    LoadTransactionHistory(canceled);
                    break;
                case 5:
                    var coinbase = AllTransactions.Where(x => new List<string> { "coinbase" }.Any(y => y.Equals(x.Status.ToLower()))).ToList();
                    LoadTransactionHistory(coinbase);
                    break;
                default:
                    LoadTransactionHistory(AllTransactions);
                    break;
            }
        }

        private ObservableCollection<TransactionStatus> _transactionStatuseOptions = new ObservableCollection<TransactionStatus>()
        {
            new TransactionStatus() { Label = AppResources.ResourceManager.GetString("All")},
            new TransactionStatus() { Label = AppResources.ResourceManager.GetString("Pending")},
            new TransactionStatus() { Label = AppResources.ResourceManager.GetString("Received")},
            new TransactionStatus() { Label = AppResources.ResourceManager.GetString("Sent")},
            new TransactionStatus() { Label = AppResources.ResourceManager.GetString("Cancelled")},
            new TransactionStatus() { Label = AppResources.ResourceManager.GetString("Coinbase")},
        };

        public ObservableCollection<TransactionStatus> TransactionStatusOptions
        {
            get
            {
                return _transactionStatuseOptions;
            }
            set
            {
                SetProperty(ref _transactionStatuseOptions, value);
            }
        }

        public Balance Balance
        {
            get
            {
                return _balance;
            }
            set
            {
                SetProperty(ref _balance, value);
            }
        }

        private bool _userCanSend = false;
        public bool UserCanSend
        {
            get
            {
                return _userCanSend;
            }
            set
            {
                SetProperty(ref _userCanSend, value);
            }
        }

        private string _wallet = "";
        public string Wallet
        {
            get
            {
                return _wallet;
            }
            set
            {
                SetProperty(ref _wallet, value);
            }
        }

        private string _statusIcon = "baseline_wifi_white_24.png";
        public string StatusIcon
        {
            get
            {
                return _statusIcon;
            }
            set
            {
                SetProperty(ref _statusIcon, value);
            }
        }

        private string _slatepackAddress = string.Empty;
        public string SlatepackAddress
        {
            get
            {
                return _slatepackAddress;
            }
            set
            {
                SetProperty(ref _slatepackAddress, value);
            }
        }

        private Transaction _selectedUnfinalizedTransaction;
        public Transaction SelectedUnfinalizedTransaction
        {
            get
            {
                return _selectedUnfinalizedTransaction;
            }
            set
            {
                if (value != null)
                {
                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        await NavigationService.NavigateAsync("TransactionDetailsPage", new NavigationParameters {
                        {
                          "transaction",
                          value
                        }
                      });
                    });
                }
                SetProperty(ref _selectedUnfinalizedTransaction, value);
            }
        }

        private Transaction _selectedFilteredTransaction;
        public Transaction SelectedFilteredTransaction
        {
            get
            {
                return _selectedFilteredTransaction;
            }
            set
            {
                if (value != null)
                {
                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        await NavigationService.NavigateAsync("TransactionDetailsPage", new NavigationParameters {
                        {
                          "transaction",
                          value
                        }
                      });
                    });
                }
                SetProperty(ref _selectedFilteredTransaction, value);
            }
        }

        public DelegateCommand<object> CancelTransactionCommand { get; private set; }

        public DelegateCommand SendButtonClickedCommand => new DelegateCommand(SendButtonClicked);

        public DelegateCommand ShareAddressCommand => new DelegateCommand(ShareAddress);

        private async void ShareAddress()
        {
            await Share.RequestAsync(new ShareTextRequest
            {
                Text = SlatepackAddress,
                Title = "grin"
            });
        }

        public WalletPageViewModel(INavigationService navigationService, IDataProvider dataProvider, IDialogService dialogService,
            IPageDialogService pageDialogService) : base(navigationService, dataProvider, dialogService, pageDialogService)
        {
            Balance = new Balance
            {
                Spendable = Preferences.Get("balance_spendable", 0.0),
                Locked = Preferences.Get("balance_locked", 0.0),
                Immature = Preferences.Get("balance_immature", 0.0),
                Unconfirmed = Preferences.Get("balance_unconfirmed", 0.0),
                Total = Preferences.Get("balance_total", 0.0)
            };

            UserCanSend = Balance.Spendable > 0;

            CancelTransactionCommand = new DelegateCommand<object>(CancelTransaction);

            Task.Factory.StartNew(async () =>
            {
                Wallet = await SecureStorage.GetAsync("username");
                SlatepackAddress = await SecureStorage.GetAsync("slatepack_address");

                await UpdateRecheability();
            });

            Task.Factory.StartNew(async () =>
            {
                await GetWalletBalance();
            });

            Task.Factory.StartNew(async () =>
            {
                await GetUnfinalizedTransactions();
            });

            Task.Factory.StartNew(async () =>
            {
                await GetTransactions();
            });
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            Device.StartTimer(TimeSpan.FromSeconds(35), () =>
            {
                if (!Settings.IsLoggedIn)
                {
                    return false;
                }

                Task.Factory.StartNew(async () =>
                {
                    await UpdateRecheability();
                });

                return true;
            });

            Device.StartTimer(TimeSpan.FromSeconds(4), () =>
            {
                if (Settings.IsLoggedIn == false)
                {
                    return false;
                }

                Task.Factory.StartNew(async () =>
                {
                    await GetWalletBalance();
                });

                return true;
            });

            Device.StartTimer(TimeSpan.FromSeconds(2), () =>
            {
                if (Settings.IsLoggedIn == false)
                {
                    return false;
                }

                Task.Factory.StartNew(async () =>
                {
                    await GetUnfinalizedTransactions();
                });

                Task.Factory.StartNew(async () =>
                {
                    await GetTransactions();
                });

                return true;
            });

            Device.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
                if (Settings.IsLoggedIn == false)
                {
                    return false;
                }

                if (!Settings.IsNodeRunning)
                {
                    Logout();
                }

                return true;
            });
        }

        private async Task UpdateRecheability()
        {
            var address = await SecureStorage.GetAsync("tor_address");

            if (string.IsNullOrEmpty(address))
            {
                Settings.Reachable = false;
                return;
            }
            try
            {
                Settings.Reachable = await DataProvider.CheckAddressAvailability(address, Settings.GrinChckAPIURL).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Settings.Reachable = false;
                Debug.WriteLine(ex.Message);
            }
        }

        private async Task GetWalletBalance()
        {
            try
            {
                var balance = await DataProvider.GetWalletBalance(await SecureStorage.GetAsync("token")).ConfigureAwait(false);
                if (Balance.Total != balance.Total || Balance.Spendable != balance.Spendable || Balance.Immature != balance.Immature ||
                    Balance.Unconfirmed != balance.Unconfirmed || Balance.Locked != balance.Locked)
                {
                    Balance = balance;
                    UserCanSend = Balance.Spendable > 0;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private async Task GetUnfinalizedTransactions()
        {
            try
            {
                string token = await SecureStorage.GetAsync("token");
                string[] filter = new string[] { "SENDING_NOT_FINALIZED" };
                List<Transaction> unfinalized = await DataProvider.GetTransactions(token, filter).ConfigureAwait(false);
                LoadUnfinalizedTransactions(unfinalized);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private async Task GetTransactions()
        {
            try
            {
                AllTransactions = await DataProvider.GetTransactions(await SecureStorage.GetAsync("token").ConfigureAwait(false), new string[] {
                  "RECEIVING_IN_PROGRESS",
                  "SENDING_FINALIZED",
                  "COINBASE",
                  "SENT",
                  "RECEIVED",
                  "SENT_CANCELED",
                  "RECEIVED_CANCELED"
                }).ConfigureAwait(false);

                FilterTransactions(CurrentSelectedFilterIndex);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void LoadUnfinalizedTransactions(List<Transaction> transactions)
        {
            if (UnfinalizedTransactions.Count() == transactions.Count())
            {
                return;
            }

            UnfinalizedTransactions = new ObservableCollection<Transaction>();

            foreach (var transaction in transactions)
            {
                UnfinalizedTransactions.Add(transaction);
            }
        }

        private void LoadTransactionHistory(List<Transaction> transactions)
        {
            if (transactions.Count() == 0)
            {
                FilteredTransactionHistory = new ObservableCollection<Transaction>();
                return;
            }

            var remove = new List<Transaction>();
            var add = new List<Transaction>();

            for (int i = 0; i < FilteredTransactionHistory.Count(); i++)
            {
                var oldTransaction = FilteredTransactionHistory.ElementAt<Transaction>(i);
                if (transactions.Exists(t => t.Id.Equals(oldTransaction.Id)))
                {
                    var newTransaction = transactions.First<Transaction>(t => t.Id.Equals(FilteredTransactionHistory.ElementAt<Transaction>(i).Id));
                    if (!newTransaction.Status.Equals(oldTransaction.Status))
                    {
                        remove.Add(oldTransaction);
                    }
                }
                else
                {
                    remove.Add(oldTransaction);
                }
            }

            for (int i = 0; i < transactions.Count(); i++)
            {
                var newTransaction = transactions.ElementAt<Transaction>(i);
                if (FilteredTransactionHistory.Any<Transaction>(t => t.Id.Equals(transactions.ElementAt<Transaction>(i).Id)))
                {
                    var oldTransaction = FilteredTransactionHistory.First<Transaction>(t => t.Id.Equals(transactions.ElementAt<Transaction>(i).Id));
                    if (!newTransaction.Status.Equals(oldTransaction.Status))
                    {
                        remove.Add(oldTransaction);
                        add.Add(newTransaction);
                    }
                }
                else
                {
                    add.Add(newTransaction);
                }
            }

            if (add.Count() == 0 && remove.Count() == 0)
            {
                return;
            }

            var newList = FilteredTransactionHistory.ToList();

            foreach (var transaction in add)
            {
                newList.Insert(0, transaction);
            }

            foreach (var transaction in remove)
            {
                newList.Remove(transaction);
            }

            FilteredTransactionHistory = new ObservableCollection<Transaction>(newList.OrderByDescending(o => o.Date));
        }

        void SendButtonClicked()
        {
            if (UserCanSend)
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await NavigationService.NavigateAsync("SetAmountPage", new NavigationParameters {
                      {
                        "spendable",
                        Balance.Spendable / Math.Pow(10, 9)
                      }
                    });
                });
            }
        }

        async void CancelTransaction(object id)
        {
            if (await PageDialogService.DisplayAlertAsync(AppResources.ResourceManager.GetString("CancelTransaction"),
                                                          AppResources.ResourceManager.GetString("CancelTransactionQuestion"),
                                                          AppResources.ResourceManager.GetString("Yes"), AppResources.ResourceManager.GetString("No")))
            {
                try
                {
                    await DataProvider.CancelTransaction(await SecureStorage.GetAsync("token").ConfigureAwait(false), (int)id).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);

                    await PageDialogService.DisplayAlertAsync("Error", ex.Message, "OK");
                }
            }
        }

        public DelegateCommand LogoutButtonClickedCommand => new DelegateCommand(Logout);

        async void Logout()
        {
            string wallet = await SecureStorage.GetAsync("username");

            try
            {
                var token = await SecureStorage.GetAsync("token");
                await DataProvider.DoLogout(token);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            finally
            {
                Preferences.Clear();
                SecureStorage.RemoveAll();

                Settings.IsLoggedIn = false;

                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await NavigationService.NavigateAsync("/NavigationPage/WalletLoginPage", new NavigationParameters { { "username", wallet } });
                });
            }
        }

        public class TransactionStatus
        {
            public string Label { get; set; }
        }
    }
}