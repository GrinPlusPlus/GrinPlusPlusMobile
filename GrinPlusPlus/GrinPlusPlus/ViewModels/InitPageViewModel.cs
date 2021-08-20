using GrinPlusPlus.Api;
using GrinPlusPlus.Models;
using GrinPlusPlus.Resources;
using Prism.Navigation;
using Prism.Services;
using Prism.Services.Dialogs;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace GrinPlusPlus.ViewModels
{
    public class InitPageViewModel : ViewModelBase
    {
        private string status = Settings.Node.Status;
        public string Status
        {
            get { return status; }
            set { SetProperty(ref status, value); }
        }

        private double _progressBar = Settings.Node.ProgressPercentage;
        public double ProgressBarr
        {
            get { return _progressBar; }
            set { SetProperty(ref _progressBar, value); }
        }

        private string _progressPercentage = string.Format($"{ Settings.Node.ProgressPercentage * 100:F}");
        public string ProgressPercentage
        {
            get { return _progressPercentage; }
            set { SetProperty(ref _progressPercentage, value); }
        }

        private ObservableCollection<Fact> _grinFacts = new ObservableCollection<Fact>();
        public ObservableCollection<Fact> GrinFacts
        {
            get { return _grinFacts; }
            set { SetProperty(ref _grinFacts, value); }
        }

        public InitPageViewModel(INavigationService navigationService, IDataProvider dataProvider, IDialogService dialogService,
            IPageDialogService pageDialogService) : base(navigationService, dataProvider, dialogService, pageDialogService)
        {
            SecureStorage.RemoveAll();

            Settings.IsLoggedIn = false;
            Settings.Node.Status = "Initializing Node...";
            Settings.Node.ProgressPercentage = 0;

            Status = Settings.Node.Status;
            ProgressBarr = Settings.Node.ProgressPercentage;
            ProgressPercentage = string.Format($"{ Settings.Node.ProgressPercentage * 100:F}");

            Device.StartTimer(TimeSpan.FromSeconds(10), () =>
            {
                Status = Settings.Node.Status;
                ProgressBarr = Settings.Node.ProgressPercentage;
                ProgressPercentage = string.Format($"{ Settings.Node.ProgressPercentage * 100:F}");

                return !Settings.Node.Status.Equals("Running");
            });

            Device.StartTimer(TimeSpan.FromSeconds(3), () =>
            {
                if (Settings.Node.Status.Equals("Running"))
                {
                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        await NavigationService.NavigateAsync("/NavigationPage/LoginPage");
                    });

                    return false;
                }

                return true;
            });

            Task.Factory.StartNew(() =>
            {
                GrinFacts.Add(new Fact { Title = AppResources.ResourceManager.GetString("Mimblewimble"), Details = AppResources.ResourceManager.GetString("FactsMimblewimble") });
                GrinFacts.Add(new Fact { Title = AppResources.ResourceManager.GetString("Addresses"), Details = AppResources.ResourceManager.GetString("FactAddresses") });
                GrinFacts.Add(new Fact { Title = AppResources.ResourceManager.GetString("Amounts"), Details = AppResources.ResourceManager.GetString("FactsAmounts") });
                GrinFacts.Add(new Fact { Title = AppResources.ResourceManager.GetString("Emission"), Details = AppResources.ResourceManager.GetString("FactEmission") });
                GrinFacts.Add(new Fact { Title = AppResources.ResourceManager.GetString("Slatepack"), Details = AppResources.ResourceManager.GetString("FactSlatepack") });
                GrinFacts.Add(new Fact { Title = AppResources.ResourceManager.GetString("Transactions"), Details = AppResources.ResourceManager.GetString("FactTransactions") });
                GrinFacts.Add(new Fact { Title = AppResources.ResourceManager.GetString("Private"), Details = AppResources.ResourceManager.GetString("FactPrivate") });
                GrinFacts.Add(new Fact { Title = AppResources.ResourceManager.GetString("Scalable"), Details = AppResources.ResourceManager.GetString("FactsScalable") });
                GrinFacts.Add(new Fact { Title = AppResources.ResourceManager.GetString("Open"), Details = AppResources.ResourceManager.GetString("FactsOpen") });
                GrinFacts.Add(new Fact { Title = AppResources.ResourceManager.GetString("Dandelion"), Details = AppResources.ResourceManager.GetString("FactsDandelion") });
            });

            Device.StartTimer(TimeSpan.FromSeconds(60), () =>
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    if (Settings.Node.Status.Equals("Not Running"))
                    {
                        await NavigationService.NavigateAsync("/NavigationPage/ErrorPage");
                    }
                });

                return false;
            });
        }
    }
}
