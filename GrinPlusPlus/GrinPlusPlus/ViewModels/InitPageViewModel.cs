using GrinPlusPlus.Api;
using Prism.Navigation;
using Prism.Services;
using Prism.Services.Dialogs;
using System;
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

        public InitPageViewModel(INavigationService navigationService, IDataProvider dataProvider, IDialogService dialogService, IPageDialogService pageDialogService)
            : base(navigationService, dataProvider, dialogService, pageDialogService)
        {
            Preferences.Clear();
            SecureStorage.RemoveAll();

            Device.StartTimer(TimeSpan.FromSeconds(0), () =>
            {
                Status = Settings.Node.Status;

                ProgressBarr = Settings.Node.ProgressPercentage;

                ProgressPercentage = string.Format($"{ Settings.Node.ProgressPercentage * 100:F}");

                if (Settings.Node.Status.Equals("Running"))
                {
                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        string destination = "InitPage";
                        
                        if (Settings.IsLoggedIn)
                        {
                            destination = "OpeningWalletPage";
                        }
                        else
                        {
                            destination = "/SharedTransitionNavigationPage/LoginPage";
                        }

                        await NavigationService.NavigateAsync(destination);
                    });

                    return false;
                }

                return true;
            });
        }
    }
}
