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
    public class WalletSeedPageViewModel : ViewModelBase
    {
        private string _walletSeed = "";
        public string WalletSeed
        {
            get => _walletSeed.Trim();
            set => SetProperty(ref _walletSeed, value);
        }

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


        public WalletSeedPageViewModel(INavigationService navigationService, IDataProvider dataProvider, IDialogService dialogService, IPageDialogService pageDialogService)
            : base(navigationService, dataProvider, dialogService, pageDialogService)
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                WalletSeed = await SecureStorage.GetAsync("wallet_seed");
                WalletSeedWordsList = new ObservableCollection<string>(WalletSeed.Split(' ').ToList());
            });
        }
    }
}
