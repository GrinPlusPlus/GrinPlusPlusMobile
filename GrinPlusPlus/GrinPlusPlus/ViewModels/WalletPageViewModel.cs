using GrinPlusPlus.Api;
using GrinPlusPlus.Models;
using GrinPlusPlus.Resources;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using Prism.Services.Dialogs;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Xamarin.Essentials;
using Xamarin.Forms;


namespace GrinPlusPlus.ViewModels
{
    public class WalletPageViewModel : ViewModelBase
    {
        private ObservableCollection<Transaction> _transactions;
        public ObservableCollection<Transaction> Transactions
        {
            get { return _transactions; }
            set { SetProperty(ref _transactions, value); }
        }

        private Balance _balance;
        public Balance Balance
        {
            get { return _balance; }
            set { SetProperty(ref _balance, value); }
        }

        private bool _userCanSend = false;
        public bool UserCanSend
        {
            get { return _userCanSend; }
            set { SetProperty(ref _userCanSend, value); }
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

        public DelegateCommand LogoutCommand => new DelegateCommand(Logout);

        public DelegateCommand OpenTransactionDetailsCommand => new DelegateCommand(OpenTransactionDetails);

        public DelegateCommand<object> CancelTransactionClickedCommand { get; private set; }

        public DelegateCommand SendButtonClickedCommand => new DelegateCommand(SendButtonClicked);

        public DelegateCommand ReceiveButtonClickedCommand => new DelegateCommand(ReceiveButtonClicked);

        public DelegateCommand FinalizeTransactionClickedCommand => new DelegateCommand(FinalizeTransactionClicked);


        public WalletPageViewModel(INavigationService navigationService, IDataProvider dataProvider, IDialogService dialogService, IPageDialogService pageDialogService)
            : base(navigationService, dataProvider, dialogService, pageDialogService)
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

            Transactions = new ObservableCollection<Transaction>();

            CancelTransactionClickedCommand = new DelegateCommand<object>(CancelTransactionClicked);

            Device.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    try
                    {
                        var balance = await DataProvider.GetWalletBalance(await SecureStorage.GetAsync("token"));
                        if (Balance.Total != balance.Total ||
                            Balance.Spendable != balance.Spendable ||
                            Balance.Immature != balance.Immature ||
                            Balance.Unconfirmed != balance.Unconfirmed ||
                            Balance.Locked != balance.Locked)
                        {
                            Balance = balance;
                            UserCanSend = Balance.Spendable > 0;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                });
                return Settings.IsLoggedIn;
            });

            Device.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    try
                    {
                        var transactions = await DataProvider.GetTransactions(await SecureStorage.GetAsync("token"),
                            new string[] { "SENDING_NOT_FINALIZED", "RECEIVING_IN_PROGRESS", "SENDING_FINALIZED" });
                        if (Transactions.Count < transactions.Count)
                        {
                            foreach (Transaction transaction in transactions)
                            {
                                if (Transactions.Any(t => t.Id == transaction.Id))
                                {
                                    continue;
                                }
                                Transactions.Add(transaction);
                            }
                        }
                        else if (Transactions.Count > transactions.Count)
                        {
                            foreach (Transaction transaction in Transactions)
                            {
                                if (transactions.Any(t => t.Id == transaction.Id))
                                {
                                    continue;
                                }
                                Transactions.Remove(transaction);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                });
                return Settings.IsLoggedIn;
            });
        }

        async void SendButtonClicked()
        {
            if (UserCanSend)
            {
                await NavigationService.NavigateAsync("SetAmountPage", new NavigationParameters { { "spendable", Balance.Spendable / Math.Pow(10, 9) } });
            }
        }

        async void ReceiveButtonClicked()
        {
            await NavigationService.NavigateAsync("ReceiveTransactionPage");
        }


        async void FinalizeTransactionClicked()
        {
            await NavigationService.NavigateAsync("FinalizeTransactionPage");
        }

        async void CancelTransactionClicked(object id)
        {
            var cancelTransactionLabel = AppResources.ResourceManager.GetString("CancelTransaction");
            var cancelTransactionMessage = AppResources.ResourceManager.GetString("CancelTransactionQuestion");
            var cancelTransactionYes = AppResources.ResourceManager.GetString("Yes");
            var cancelTransactionNo = AppResources.ResourceManager.GetString("No");

            if (await PageDialogService.DisplayAlertAsync(cancelTransactionLabel, cancelTransactionMessage, cancelTransactionYes, cancelTransactionNo))
            {
                try
                {
                    await DataProvider.CancelTransaction(await SecureStorage.GetAsync("token"), (int)id);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }
        }

        async void OpenTransactionDetails()
        {
            if (SelectedTransaction is null) return;
            await NavigationService.NavigateAsync("TransactionDetailsPage", new NavigationParameters { { "transaction", SelectedTransaction } });
            SelectedTransaction = null;
        }

        async void Logout()
        {
            try
            {
                await DataProvider.DoLogout(await SecureStorage.GetAsync("token"));
                var torAddress = await SecureStorage.GetAsync("tor_address");
                if (!string.IsNullOrEmpty(torAddress))
                {
                    await DataProvider.DeleteONION(torAddress);
                    SecureStorage.Remove("tor_address");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Settings.IsLoggedIn = false;
            
            Preferences.Clear();
            SecureStorage.RemoveAll();

            await NavigationService.NavigateAsync("/SharedTransitionNavigationPage/LoginPage");
        }
    }
}
