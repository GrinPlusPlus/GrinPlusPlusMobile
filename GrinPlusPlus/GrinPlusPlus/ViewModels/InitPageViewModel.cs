using GrinPlusPlus.Api;
using GrinPlusPlus.Models;
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
        private NodeStatus status;
        public NodeStatus Status
        {
            get { return status; }
            set { SetProperty(ref status, value); }
        }

        private int _progressPercentage = 0 ;
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
                        Status = await DataProvider.GetNodeStatus();
                        ProgressPercentage = (int)Math.Round(Status.ProgressPercentage*100.0);
                        if (Status.SyncStatus.Equals("Running"))
                        {
                            await Task.Delay(TimeSpan.FromSeconds(1));
                            await NavigationService.NavigateAsync("/SharedTransitionNavigationPage/LoginPage");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                });

                var status = Status == null ? "offline" : Status.SyncStatus;
                return status.Equals("Running");
            });
        }

    }
}
