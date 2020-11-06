using GrinPlusPlus.Models;
using GrinPlusPlus.Services;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace GrinPlusPlus.ViewModels
{
    public class TansactionsHistoryPageViewModel :  ViewModelBase
    {
        public ObservableCollection<Transaction> Transactions { get; set; }

        public TansactionsHistoryPageViewModel(INavigationService navigationService, IDataProvider dataProvider)
            : base(navigationService)
        {
            string[] inProgress = { "Receiving", "Sending" };
            Transactions = new ObservableCollection<Transaction>();
            foreach (var transaction in dataProvider.GetAllData())
            {
                if (!inProgress.Contains(transaction.Status))
                {
                    Transactions.Add(transaction);
                }
            }
        }
    }
}
