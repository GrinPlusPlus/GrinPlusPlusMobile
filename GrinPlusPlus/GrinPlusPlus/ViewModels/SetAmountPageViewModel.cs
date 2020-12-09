using GrinPlusPlus.Api;
using GrinPlusPlus.Models;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using Prism.Services.Dialogs;
using System;
using System.Globalization;
using System.Linq;
using Xamarin.Essentials;

namespace GrinPlusPlus.ViewModels
{
    public class SetAmountPageViewModel : ViewModelBase
    {
        public DelegateCommand<string> KeyboardButtonClickedCommand => new DelegateCommand<string>(KeyboardButtonClicked);

        public DelegateCommand MaxButtonClickedCommand => new DelegateCommand(MaxButtonClicked);

        public DelegateCommand ContinueButtonClickedCommand => new DelegateCommand(ContinueButtonClicked);

        private double _spendable;
        public double Spendable
        {
            get { return _spendable; }
            set { SetProperty(ref _spendable, value); }
        }

        private string _spendableLabel;
        public string SpendableLabel
        {
            get { return _spendableLabel; }
            set { SetProperty(ref _spendableLabel, value); }
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

        private double _dAmount
        {
            get { return Helpers.TextCurrencyToDouble(_amount); }
        }

        private string _fee = "-";
        public string Fee
        {
            get { return _fee; }
            set { SetProperty(ref _fee, value); }
        }

        public CultureInfo Culture { get; private set; }

        public SetAmountPageViewModel(INavigationService navigationService, IDataProvider dataProvider, IDialogService dialogService, IPageDialogService pageDialogService)
            : base(navigationService, dataProvider, dialogService, pageDialogService)
        {
            Culture = new CultureInfo("en-US");
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            if (parameters.ContainsKey("spendable"))
            {
                Spendable = (double)parameters["spendable"];
                SpendableLabel = $"MAX: {Spendable} ツ";
            }
        }

        void KeyboardButtonClicked(string character)
        {
            var numbers = new string[10] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };

            if (numbers.Contains(character))
            {
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


        void MaxButtonClicked()
        {
            Amount = $"{Spendable}";
            Fee = "-";
            IsValid = true;
        }

        async void ContinueButtonClicked()
        {
            try
            {
                var amount = Helpers.TextCurrencyToDouble(Amount);

                await NavigationService.NavigateAsync("EnterAddressMessagePage", new NavigationParameters { { "amount", amount }, { "max", (Helpers.TextCurrencyToDouble(Amount) == Spendable) } });
            }
            catch (Exception ex)
            {
                await PageDialogService.DisplayAlertAsync("Error", ex.Message, "OK");
            }
        }

        private void TriggerFeeCalculation()
        {
            if (Amount.LastIndexOf(".").Equals(Amount.Length - 1))
            {
                IsValid = false;
                Fee = "-";
                return;
            }
            else if (_dAmount.Equals(0))
            {
                IsValid = false;
                Fee = "-";
                return;
            }
            else
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    try
                    {
                        FeeEstimation estimation = await DataProvider.EstimateFee(await SecureStorage.GetAsync("token"), _dAmount);
                        Fee = (estimation.Fee / Math.Pow(10, 9)).ToString("F9");
                        IsValid = true;
                    }
                    catch (Exception ex)
                    {
                        IsValid = false;
                        Fee = "-";
                        await PageDialogService.DisplayAlertAsync("Error", ex.Message, "OK");
                    }
                });
            }
        }
    }
}
