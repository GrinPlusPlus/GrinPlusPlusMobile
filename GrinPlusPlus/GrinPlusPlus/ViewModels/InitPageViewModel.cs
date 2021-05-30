using GrinPlusPlus.Api;
using GrinPlusPlus.Models;
using GrinPlusPlus.Resources;
using GrinPlusPlus.Extensions;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using Prism.Services.Dialogs;
using System;
using System.Collections.ObjectModel;
using System.Threading;
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

        private bool displayActivityIndicator = true;
        public bool DisplayActivityIndicator
        {
            get { return displayActivityIndicator; }
            set { SetProperty(ref displayActivityIndicator, value); }
        }

        private ObservableCollection<Fact> _grinFacts = new ObservableCollection<Fact>();
        public ObservableCollection<Fact> GrinFacts
        {
            get { return _grinFacts; }
            set { SetProperty(ref _grinFacts, value); }
        }

        public InitPageViewModel(INavigationService navigationService, IDataProvider dataProvider, IDialogService dialogService, IPageDialogService pageDialogService)
            : base(navigationService, dataProvider, dialogService, pageDialogService)
        {
            Preferences.Clear();
            SecureStorage.RemoveAll();

            FillGrinFacts();

            Device.StartTimer(TimeSpan.FromSeconds(2), () =>
            {
                Status = Settings.Node.Status;
                ProgressBarr = Settings.Node.ProgressPercentage;
                ProgressPercentage = string.Format($"{ Settings.Node.ProgressPercentage * 100:F}");

                if (Settings.Node.Status.Equals("Running"))
                {
                    DisplayActivityIndicator = false;

                    return false;
                } else
                
                return true;
            });

            Device.StartTimer(TimeSpan.FromSeconds(30), () =>
            {
                if (Settings.Node.Status.Equals("Disconnected"))
                {
                    DisplayActivityIndicator = false;

                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        await NavigationService.NavigateAsync("/SharedTransitionNavigationPage/ErrorPage");
                    });
                }
                return false;
            });

            Device.StartTimer(TimeSpan.FromSeconds(5), () =>
            {
                if (Settings.Node.Status.Equals("Running"))
                {
                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        await NavigationService.NavigateAsync("/SharedTransitionNavigationPage/LoginPage");
                    });

                    return false;
                }

                return true;
            });
        }

        private void FillGrinFacts()
        {
            _grinFacts.Add(new Fact{ Title = AppResources.ResourceManager.GetString("Emission"), Details = AppResources.ResourceManager.GetString("FactEmission") });
            _grinFacts.Add(new Fact { Title = AppResources.ResourceManager.GetString("Slatepack"), Details = AppResources.ResourceManager.GetString("FactSlatepack") });
            _grinFacts.Add(new Fact { Title = AppResources.ResourceManager.GetString("Transactions"), Details = AppResources.ResourceManager.GetString("FactTransactions") });
            _grinFacts.Add(new Fact { Title = AppResources.ResourceManager.GetString("Addresses"), Details = AppResources.ResourceManager.GetString("FactAddresses") });
            _grinFacts.Add(new Fact { Title = AppResources.ResourceManager.GetString("Private"), Details = AppResources.ResourceManager.GetString("FactPrivate") });
            _grinFacts.Add(new Fact { Title = AppResources.ResourceManager.GetString("Scalable"), Details = AppResources.ResourceManager.GetString("FactsScalable") });
            _grinFacts.Add(new Fact { Title = AppResources.ResourceManager.GetString("Amounts"), Details = AppResources.ResourceManager.GetString("FactsAmounts") });
            _grinFacts.Add(new Fact { Title = AppResources.ResourceManager.GetString("Open"), Details = AppResources.ResourceManager.GetString("FactsOpen") });
            _grinFacts.Add(new Fact { Title = AppResources.ResourceManager.GetString("Dandelion"), Details = AppResources.ResourceManager.GetString("FactsDandelion") });
            _grinFacts.Add(new Fact { Title = AppResources.ResourceManager.GetString("Mimblewimble"), Details = AppResources.ResourceManager.GetString("FactsMimblewimble") });

            _grinFacts.Shuffle();
        }
    }
}
