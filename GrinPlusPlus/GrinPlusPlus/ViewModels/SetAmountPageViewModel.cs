using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GrinPlusPlus.ViewModels
{
    public class SetAmountPageViewModel : ViewModelBase
    {
        public DelegateCommand<string> KeyboardButtonClickedCommand => new DelegateCommand<string>(KeyboardButtonClicked);

        private bool isValid;
        public bool IsValid
        {
            get { return isValid; }
            set
            {
                SetProperty(ref isValid, value);
            }
        }

        private string _amount;
        public string Amount
        {
            get { return _amount; }
            set { SetProperty(ref _amount, value); }
        }

        private string _fee;
        public string Fee
        {
            get { return _fee; }
            set { SetProperty(ref _fee, value); }
        }

        public SetAmountPageViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            Amount = "0";
            Fee = "0.000000000";
        }

        void KeyboardButtonClicked(string character)
        {
            if(character.Equals("DEL") && Amount.Length > 0)
            {
                Amount = Amount.Remove(Amount.Length - 1, 1);
                if(Amount.Length == 0)
                {
                    Amount = "0";
                    Fee = "0.000000000";
                }
                return;
            }
            if(character.Equals(".") && Amount.Contains("."))
            {
                return;
            }
            if (Amount.Equals("0"))
            {
                Amount = "";
            }
            Amount = $"{Amount}{character}";
        }

    }
}
