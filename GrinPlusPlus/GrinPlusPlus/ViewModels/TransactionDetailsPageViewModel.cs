using GrinPlusPlus.Models;
using GrinPlusPlus.Services;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Xamarin.Essentials;


namespace GrinPlusPlus.ViewModels
{
    public class TransactionDetailsPageViewModel : ViewModelBase
    {
        private decimal _amount;
        public decimal Amount
        {
            get { return Math.Truncate(_amount); }
            set { SetProperty(ref _amount, value); }
        }

        private double _fee;
        public double Fee
        {
            get { return _fee; }
            set { SetProperty(ref _fee, value); }
        }

        private double _decimals;
        public double Decimals
        {
            get { return _decimals; }
            set { SetProperty(ref _decimals, value); }
        }

        private string _status;
        public string Status
        {
            get { return _status; }
            set { SetProperty(ref _status, value); }
        }

        private DateTime _date;
        public DateTime Date
        {
            get { return _date; }
            set { SetProperty(ref _date, value); }
        }

        private string _slate;
        public string Slate
        {
            get { return _slate; }
            set { SetProperty(ref _slate, value); }
        }

        private List<Output> _outputs;
        public List<Output> Outputs
        {
            get { return _outputs; }
            set { SetProperty(ref _outputs, value); }
        }

        private string _message;
        public string Message
        {
            get { return _message; }
            set { SetProperty(ref _message, value); }
        }


        public TransactionDetailsPageViewModel(INavigationService navigationService)
            : base(navigationService)
        {

        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            if (parameters.ContainsKey("transaction"))
            {
                Transaction transaction = (Transaction)parameters["transaction"];
                Amount = transaction.Amount;
                Decimals = transaction.Decimals;
                Status = transaction.Status.ToUpper();
                Fee = transaction.Fee;
                Date = transaction.Date;
                Slate = transaction.Slate;
                Message = transaction.Message;
                Outputs = transaction.Outputs;
            }
        }
    }
}
