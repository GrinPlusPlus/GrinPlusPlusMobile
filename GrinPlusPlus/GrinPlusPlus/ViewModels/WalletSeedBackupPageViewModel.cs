using GrinPlusPlus.Api;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Collections.ObjectModel;
using Xamarin.Essentials;
using System.Linq;


namespace GrinPlusPlus.ViewModels
{
    public class WalletSeedBackupPageViewModel : ViewModelBase
    {
        private ObservableCollection<string> _walletSeedWordsList;
        public ObservableCollection<string> WalletSeedWordsList
        {
            get
            {
                return _walletSeedWordsList;
            }
            set
            {
                SetProperty(ref _walletSeedWordsList, value);
            }
        }

        public WalletSeedBackupPageViewModel (INavigationService navigationService, IDataProvider dataProvider, IDialogService dialogService, IPageDialogService pageDialogService)
            : base(navigationService, dataProvider, dialogService, pageDialogService)
        {

        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            if (parameters.ContainsKey("wallet_seed"))
            {
                string seed = parameters.GetValue<string>("wallet_seed");
                WalletSeedWordsList = new ObservableCollection<string>(seed.Split(' ').ToList());
            }
        }
    }
}
