using GrinPlusPlus.Api;
using GrinPlusPlus.Models;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            get { return _amount; }
            set { SetProperty(ref _amount, value); }
        }

        private string _fee = "0.000000000";
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

        private List<Output> _inputs;
        public List<Output> Inputs
        {
            get { return _inputs; }
            set { SetProperty(ref _inputs, value); }
        }

        public SetAmountPageViewModel(INavigationService navigationService, IDataProvider dataProvider, IDialogService dialogService, IPageDialogService pageDialogService)
            : base(navigationService, dataProvider, dialogService, pageDialogService)
        {

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
            SendMax = false;
            if (character.Equals("DEL") && Amount.Length > 0)
            {
                Amount = Amount.Remove(Amount.Length - 1, 1).Trim();
                if (string.IsNullOrEmpty(Amount))
                {
                    Amount = "0";
                    Fee = "0.000000000";
                    IsValid = false;
                }
                IsValid = !Amount.Trim().Equals("0");
                return;
            }
            if (character.Equals(".") && Amount.Contains("."))
            {
                return;
            }

            if (character.Equals("0") && Amount.Equals("0"))
            {
                IsValid = false;
                return;
            }

            Amount = $"{Amount}{character}";

            if (Amount.Equals("."))
            {
                Amount = "0.";
                Fee = "0.000000000";
                IsValid = false;
                return;
            }

            if(!character.Equals(".") && Amount.Equals($"0{character}"))
            {
                Amount = $"{character}";
            }

            IsValid = !Amount.Trim().Equals("0") && !Amount.Trim().Equals("0.");

            if (Double.Parse(Amount) > Spendable)
            {
                IsValid = false;
            }

            if (Double.Parse(Amount) == Spendable)
            {
                SendMax = true;
            }

            TriggerFeeCalculation();
        }

        
        void MaxButtonClicked()
        {
            Amount = $"{Spendable}";
            IsValid = !Amount.Trim().Equals("0");
            Fee = $"0.000000000";
            if (Double.Parse(Amount) == Spendable)
            {
                SendMax = true;
            }
        }

        async void ContinueButtonClicked()
        {
            if (Double.Parse(Amount) == Spendable)
            {
                SendMax = true;
            }
            await NavigationService.NavigateAsync("EnterAddressMessagePage",
                new NavigationParameters { { "amount", Amount }, { "fee", Fee }, { "max", SendMax } });
        }

        private void TriggerFeeCalculation()
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                try
                {
                    FeeEstimation estimation = await DataProvider.EstimateFee(await SecureStorage.GetAsync("token"), Double.Parse(Amount));
                    Fee = (estimation.Fee / Math.Pow(10, 9)).ToString("F9");
                    Inputs = estimation.Inputs;
                }
                catch (Exception ex)
                {
                    await PageDialogService.DisplayAlertAsync("Error", ex.Message, "OK");
                    IsValid = false;
                }
            });
        }
    }
}
