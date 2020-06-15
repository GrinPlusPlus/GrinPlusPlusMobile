using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GrinPlusPlus.ViewModels
{
    public class InitPageViewModel : ViewModelBase
    {
        public InitPageViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            Title = "Init Page";
        }   
    }
}
