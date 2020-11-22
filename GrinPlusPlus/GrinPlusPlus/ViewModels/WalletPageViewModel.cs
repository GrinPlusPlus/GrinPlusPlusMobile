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
                Spendable = Double.Parse(Preferences.Get("balance_spendable", "0.000000000")),
                Locked = Double.Parse(Preferences.Get("balance_locked", "0.000000000")),
                Immature = Double.Parse(Preferences.Get("balance_immature", "0.000000000")),
                Unconfirmed = Double.Parse(Preferences.Get("balance_unconfirmed", "0.000000000")),
                Total = Double.Parse(Preferences.Get("balance_total", "0.000000000"))
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
                        var balance = await dataProvider.GetWalletBalance(await SecureStorage.GetAsync("token"));
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
                        Debug.WriteLine(ex.Message);
                    }
                });
                return true;
            });

            Device.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    try
                    {
                        var transactions = await dataProvider.GetTransactions(await SecureStorage.GetAsync("token"),
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
                        Debug.WriteLine(ex.Message);
                    }
                });
                return true;
            });
        }

        async void SendButtonClicked()
        {
            if (UserCanSend)
            {
                await NavigationService.NavigateAsync("SetAmountPage", new NavigationParameters { { "spendable", Balance.Spendable } });
            }
        }

        async void ReceiveButtonClicked()
        {
            var method = AppResources.ResourceManager.GetString("Method");
            var close = AppResources.ResourceManager.GetString("Close");
            var qr = AppResources.ResourceManager.GetString("ScanningQRCode");
            var text = AppResources.ResourceManager.GetString("SlatepackMessageText");
            var file = AppResources.ResourceManager.GetString("SlatepackMessageFile");
            var nfc = AppResources.ResourceManager.GetString("UsingNFC");

            var selectedMethod = await PageDialogService.DisplayActionSheetAsync(method, close, null, qr, text, file, nfc);
            if(selectedMethod == null)
            {
                return;
            }
            if (selectedMethod.Equals(qr))
            {
                await NavigationService.NavigateAsync(name:"ReceiveTransactionPage", parameters: null, useModalNavigation: true, animated: true);
            }
            else if (selectedMethod.Equals(text))
            {
                await NavigationService.NavigateAsync(name: "ReceiveTransactionPage", parameters: null, useModalNavigation: true, animated: true);
            }
            else if (selectedMethod.Equals(file))
            {
                await NavigationService.NavigateAsync(name: "ReceiveTransactionPage", parameters: null, useModalNavigation: true, animated: true);
            }
            else if (selectedMethod.Equals(nfc))
            {
                await NavigationService.NavigateAsync(name: "ReceiveTransactionPage", parameters: null, useModalNavigation: true, animated: true);
            }
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
    }
}
