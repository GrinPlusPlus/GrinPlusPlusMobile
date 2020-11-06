using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GrinPlusPlus.ViewModels
{
    public class DashboardCarouselPageViewModel : ViewModelBase
    {
        private string _wallet;
        public string Wallet
        {
            get { return _wallet; }
            set { SetProperty(ref _wallet, value); }
        }

        public DashboardCarouselPageViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            Wallet = "donations";
        }
    }
}
