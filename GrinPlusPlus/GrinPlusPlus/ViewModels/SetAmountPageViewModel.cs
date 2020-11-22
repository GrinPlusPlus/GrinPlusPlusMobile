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

        private decimal _spendable;
        public decimal Spendable
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
                Spendable = (decimal)parameters["spendable"];
                SpendableLabel = $"Spendable: {Spendable} ツ";
            }
        }

        void KeyboardButtonClicked(string character)
        {
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

            IsValid = !Amount.Trim().Equals("0") && !Amount.Trim().Equals("0.");

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
                    Debug.WriteLine(ex.Message);
                }
            });
        }

        void MaxButtonClicked()
        {
            Amount = $"{Spendable}";
            IsValid = !Amount.Trim().Equals("0");
        }

        async void ContinueButtonClicked()
        {
            List<string> inputs = new List<string>();
            foreach (Output output in Inputs)
            {
                inputs.Add(output.Commitment);
            }

            await NavigationService.NavigateAsync("EnterAddressMessagePage",
                new NavigationParameters { { "amount", Amount }, { "fee", Fee }, { "inputs", inputs.ToArray() } });
        }
    }
}
