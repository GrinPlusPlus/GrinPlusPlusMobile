using GrinPlusPlus.Api;
using Prism.Navigation;
using Prism.Services;
using Prism.Services.Dialogs;
using System;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace GrinPlusPlus.ViewModels
{
    public class InitPageViewModel : ViewModelBase
    {
        private string status = "Offline";
        public string Status
        {
            get { return status; }
            set { SetProperty(ref status, value); }
        }

        private double _progressBar = 0;
        public double ProgressBarr
        {
            get { return _progressBar; }
            set { SetProperty(ref _progressBar, value); }
        }

        private int _progressPercentage = 0;
        public int ProgressPercentage
        {
            get { return _progressPercentage; }
            set { SetProperty(ref _progressPercentage, value); }
        }


        public InitPageViewModel(INavigationService navigationService, IDataProvider dataProvider, IDialogService dialogService, IPageDialogService pageDialogService)
            : base(navigationService, dataProvider, dialogService, pageDialogService)
        {

            Device.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    try
                    {
                        var status = await DataProvider.GetNodeStatus();
                        Status = status.SyncStatus;

                        if (Status.Equals("Running"))
                        {
                            ProgressBarr = 1;
                            ProgressPercentage = 100;
                            await Task.Delay(TimeSpan.FromSeconds(1));
                            await NavigationService.NavigateAsync("/SharedTransitionNavigationPage/LoginPage");
                        }
                        else
                        {
                            ProgressBarr = status.ProgressPercentage;
                            ProgressPercentage = (int)Math.Round(ProgressBarr * 100.0);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                });

                return !Status.Equals("Running");
            });
        }

    }
}
