using GrinPlusPlus.Api;
using GrinPlusPlus.Models;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using Prism.Services.Dialogs;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace GrinPlusPlus.ViewModels
{
    public class SetAmountPageViewModel : ViewModelBase
    {
        public DelegateCommand<string> KeyboardButtonClickedCommand => new DelegateCommand<string>(KeyboardButtonClicked);

        public DelegateCommand ContinueButtonClickedCommand => new DelegateCommand(ContinueButtonClicked);

        private string _address = string.Empty;
        public string Address
        {
            get { return _address; }
            set { SetProperty(ref _address, value); }
        }

        private double _spendable;
        public double Spendable
        {
            get { return _spendable; }
            set { SetProperty(ref _spendable, value); }
        }

        private bool isValid = false;
        public bool IsValid
        {
            get { return isValid; }
            set
            {
                SetProperty(ref isValid, value);
            }
        }

        private string _amount = "0";
        public string Amount
        {
            get { return _amount.Trim(); }
            set { SetProperty(ref _amount, value); }
        }

        private double __amount
        {
            get { return Helpers.TextCurrencyToDouble(Amount); }
        }

        private string _fee = "0";
        public string Fee
        {
            get { return _fee; }
            set { SetProperty(ref _fee, value); }
        }

        private bool _sendMax = false;
        public bool SendMax
        {
            get { return _sendMax; }
            set { SetProperty(ref _sendMax, value); }
        }

        public DelegateCommand OnQRButtonClickedCommand => new DelegateCommand(OnQRButtonClicked);

        private async void OnQRButtonClicked()
        {
            await NavigationService.NavigateAsync(name: "QRScannerPage", parameters: null, useModalNavigation: true, animated: true);
        }

        public CultureInfo Culture { get; private set; }

        public SetAmountPageViewModel(INavigationService navigationService, IDataProvider dataProvider, IDialogService dialogService,
                                      IPageDialogService pageDialogService)
            : base(navigationService, dataProvider, dialogService, pageDialogService)
        {
            Culture = new CultureInfo("en-US");
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            var navigationMode = parameters.GetNavigationMode();

            switch (navigationMode)
            {
                case Prism.Navigation.NavigationMode.New:
                    if (parameters.ContainsKey("spendable"))
                    {
                        Spendable = (double)parameters["spendable"];
                    }
                    break;
                case Prism.Navigation.NavigationMode.Back:
                    if (parameters.ContainsKey("qr_scanner_result"))
                    {
                        Address = (string)parameters["qr_scanner_result"];
                    }
                    break;
            }
        }

        void KeyboardButtonClicked(string character)
        {
            var numbers = new string[10] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };

            if (numbers.Contains(character))
            {
                var parts = Amount.Split('.');
                if (parts.Length.Equals(2))
                {
                    if (parts[1].Length.Equals(9))
                    {
                        return;
                    }
                }

                if (Amount.Equals("0"))
                {
                    Amount = string.Empty;
                }

                Amount = $"{Amount}{character}";
            }
            else
            {
                if (character.Equals("."))
                {
                    if (Amount.Contains("."))
                    {
                        return;
                    }
                    Amount = $"{Amount}{character}";
                }
                else if (character.Equals("DEL"))
                {
                    Amount = Amount.Remove(Amount.Length - 1, 1).Trim();
                    if (Amount.Length == 0)
                    {
                        Amount = "0";
                    }
                }
            }

            TriggerFeeCalculation();
        }

        void ContinueButtonClicked()
        {
            try
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await NavigationService.NavigateAsync("SendingGrinsPage",
                        new NavigationParameters
                        {
                            { "address", Address },
                            { "amount", __amount },
                            { "max", SendMax }
                        }
                    );
                });

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);

                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await PageDialogService.DisplayAlertAsync("Error", ex.Message, "OK");
                });
            }
        }

        private void TriggerFeeCalculation()
        {
            if (Amount.LastIndexOf(".").Equals(Amount.Length - 1))
            {
                IsValid = false;
                Fee = "0";
                return;
            }
            else if (__amount.Equals(0))
            {
                IsValid = false;
                Fee = "0";
                return;
            }
            else if (__amount > Spendable - Helpers.TextCurrencyToDouble(Fee))
            {
                IsValid = false;
                Fee = "0";
                return;
            }

            Task.Factory.StartNew(async () =>
            {
                try
                {
                    FeeEstimation estimation = await DataProvider.EstimateFee(await SecureStorage.GetAsync("token").ConfigureAwait(false), __amount).ConfigureAwait(false);

                    Fee = (estimation.Fee / Math.Pow(10, 9)).ToString("F9");
                    IsValid = true;
                }
                catch (Exception ex)
                {
                    IsValid = false;
                    Fee = "0";

                    Debug.WriteLine(ex.Message);

                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        await PageDialogService.DisplayAlertAsync("Error", ex.Message, "OK");
                    });
                }
            });
        }
    }
}
