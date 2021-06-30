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
            FilteredTransactionHistory = new ObservableCollection<Transaction>();

            switch (value)
            {
                case 1:
                    LoadTransactionHistory(AllTransactions.Where(x => new List<string> { "sending", "receiving" }.Any(y => y == x.Status.ToLower())).ToList());
                    break;
                case 2:
                    LoadTransactionHistory(AllTransactions.Where(x => new List<string> { "received" }.Any(y => y == x.Status.ToLower())).ToList());
                    break;
                case 3:
                    LoadTransactionHistory(AllTransactions.Where(x => new List<string> { "sent" }.Any(y => y == x.Status.ToLower())).ToList());
                    break;
                case 4:
                    LoadTransactionHistory(AllTransactions.Where(x => new List<string> { "canceled" }.Any(y => y == x.Status.ToLower())).ToList());
                    break;
                case 5:
                    LoadTransactionHistory(AllTransactions.Where(x => new List<string> { "coinbase" }.Any(y => y == x.Status.ToLower())).ToList());
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

        private string _torAddress = string.Empty;
        public string TorAddress
        {
            get
            {
                return _torAddress;
            }
            set
            {
                SetProperty(ref _torAddress, value);
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
                    MainThread.BeginInvokeOnMainThread(async () => {
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
                    MainThread.BeginInvokeOnMainThread(async () => {
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

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            Device.StartTimer(TimeSpan.FromSeconds(5), () => {
                if (Settings.IsLoggedIn == false)
                {
                    return false;
                }

                MainThread.BeginInvokeOnMainThread(async () => {
                    await GetWalletBalance();
                });

                return true;
            });

            Device.StartTimer(TimeSpan.FromSeconds(5), () => {
                if (Settings.IsLoggedIn == false)
                {
                    return false;
                }

                MainThread.BeginInvokeOnMainThread(async () => {
                    await LoadTransactions();
                });

                return true;
            });

            Device.StartTimer(TimeSpan.FromSeconds(5), () => {
                if (Settings.IsLoggedIn == false)
                {
                    return false;
                }

                if (Settings.Node.Status.Equals("Not Connected"))
                {
                    StatusIcon = "baseline_wifi_off_white_24.png";
                }
                else
                {
                    StatusIcon = "baseline_wifi_white_24.png";
                }

                return true;
            });

            Wallet = await SecureStorage.GetAsync("username");
            TorAddress = await SecureStorage.GetAsync("tor_address");
            SlatepackAddress = await SecureStorage.GetAsync("slatepack_address");
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

            MainThread.BeginInvokeOnMainThread(async () => {
                await GetWalletBalance();
            });

            MainThread.BeginInvokeOnMainThread(async () => {
                await LoadTransactions();
            });

            Device.StartTimer(TimeSpan.FromSeconds(30), () => {
                if (Settings.IsLoggedIn == false)
                {
                    return false;
                }

                UpdateAvailability();

                return true;
            });
        }

        void UpdateAvailability()
        {
            MainThread.BeginInvokeOnMainThread(async () => {

                if (string.IsNullOrEmpty(TorAddress))
                {
                    Settings.Reachable = false;
                    return;
                }
                try
                {
                    Settings.Reachable = await DataProvider.CheckAddressAvailability(TorAddress, Settings.GrinChckAPIURL);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);

                    Settings.Reachable = false;
                }
            });
        }

        private async Task GetWalletBalance()
        {
            try
            {
                var balance = await DataProvider.GetWalletBalance(await SecureStorage.GetAsync("token"));
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

        private async Task LoadTransactions()
        {
            try
            {
                AllTransactions = await DataProvider.GetTransactions(await SecureStorage.GetAsync("token"), new string[] {
                  "RECEIVING_IN_PROGRESS",
                  "SENDING_FINALIZED",
                  "COINBASE",
                  "SENT",
                  "RECEIVED",
                  "SENT_CANCELED",
                  "RECEIVED_CANCELED"
                });

                LoadUnfinalizedTransactions(await DataProvider.GetTransactions(await SecureStorage.GetAsync("token"), new string[] {"SENDING_NOT_FINALIZED"}));
                FilterTransactions(CurrentSelectedFilterIndex);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void LoadUnfinalizedTransactions(List<Transaction> transactions)
        {
            if(UnfinalizedTransactions.Count() != transactions.Count())
            {
                UnfinalizedTransactions = new ObservableCollection<Transaction>();
            }

            foreach (var transaction in transactions)
            {
                try
                {
                    var current = UnfinalizedTransactions.First<Transaction>(t => t.Id.Equals(transaction.Id));
                    
                    if (!current.Status.Equals(transaction.Status))
                    {
                        UnfinalizedTransactions.Remove(current);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);

                    UnfinalizedTransactions.Add(transaction);
                }
            }
        }

        private void LoadTransactionHistory(List<Transaction> transactions)
        {
            if (FilteredTransactionHistory.Count() != transactions.Count())
            {
                FilteredTransactionHistory = new ObservableCollection<Transaction>();
            }

            foreach (var transaction in transactions)
            {
                try
                {
                    var current = FilteredTransactionHistory.First<Transaction>(t => t.Id.Equals(transaction.Id));
                    
                    if (!current.Status.Equals(transaction.Status))
                    {
                        FilteredTransactionHistory.Remove(current);
                        FilteredTransactionHistory.Add(transaction);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);

                    FilteredTransactionHistory.Add(transaction);
                }
            }
        }

        async void SendButtonClicked()
        {
            if (UserCanSend)
            {
                await NavigationService.NavigateAsync("SetAmountPage", new NavigationParameters {
                  {
                    "spendable",
                    Balance.Spendable / Math.Pow(10, 9)
                  }
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
                    await DataProvider.CancelTransaction(await SecureStorage.GetAsync("token"), (int)id);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);

                    await PageDialogService.DisplayAlertAsync("Error", ex.Message, "OK");
                }
            }
        }

       public class TransactionStatus
       {
           public string Label { get; set; }
       }
    }
}