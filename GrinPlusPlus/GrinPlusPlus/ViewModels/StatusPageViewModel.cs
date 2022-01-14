using GrinPlusPlus.Api;
using GrinPlusPlus.Models;
using Prism.Navigation;
using Prism.Services;
using Prism.Services.Dialogs;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace GrinPlusPlus.ViewModels
{
    public class StatusPageViewModel : ViewModelBase
    {
        private string isTorRunning = Settings.IsTorRunning ? "Running" : "Not Running";
        public string IsTorRunning
        {
            get { return isTorRunning; }
            set { SetProperty(ref isTorRunning, value); }
        }

        private string status = Settings.Node.Status;
        public string Status
        {
            get { return status; }
            set { SetProperty(ref status, value); }
        }

        private int headerHeight = Settings.Node.HeaderHeight;
        public int HeaderHeight
        {
            get { return headerHeight; }
            set { SetProperty(ref headerHeight, value); }
        }

        private int blocks = Settings.Node.Blocks;
        public int Blocks
        {
            get { return blocks; }
            set { SetProperty(ref blocks, value); }
        }

        private int networkHeight = Settings.Node.NetworkHeight;
        public int NetworkHeight
        {
            get { return networkHeight; }
            set { SetProperty(ref networkHeight, value); }
        }

        private ObservableCollection<Peer> _connectedPeers = new ObservableCollection<Peer>();
        public ObservableCollection<Peer> ConnectedPeers
        {
            get { return _connectedPeers; }
            set { SetProperty(ref _connectedPeers, value); }
        }

        private string _progressPercentage = string.Format($"{ Settings.Node.ProgressPercentage * 100:F}");
        public string ProgressPercentage
        {
            get { return _progressPercentage; }
            set { SetProperty(ref _progressPercentage, value); }
        }

        private bool StopTimer = false;

        public StatusPageViewModel(INavigationService navigationService, IDataProvider dataProvider, IDialogService dialogService, IPageDialogService pageDialogService)
            : base(navigationService, dataProvider, dialogService, pageDialogService)
        {
            Task.Run(() => 
            {
                UpdateStatus();
            });

            Task.Run(() =>
            {
                ListConnectedPeers();

                UpdateStatus();
            });
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            Device.StartTimer(TimeSpan.FromSeconds(2), () =>
            {
                UpdateStatus();

                return !StopTimer;
            });

            Device.StartTimer(TimeSpan.FromSeconds(5), () =>
            {
                if (StopTimer) return false;

                ListConnectedPeers();

                return !StopTimer;
            });
        }

        public override void OnNavigatedFrom(INavigationParameters parameters)
        {
            StopTimer = true;
        }

        private void UpdateStatus()
        {
            IsTorRunning = Settings.IsTorRunning ? "Running" : "Not Running";
            ProgressPercentage = string.Format($"{ Settings.Node.ProgressPercentage * 100:F}");
            Status = Settings.Node.Status;
            HeaderHeight = Settings.Node.HeaderHeight;
            Blocks = Settings.Node.Blocks;
            NetworkHeight = Settings.Node.NetworkHeight;
        }

        void ListConnectedPeers()
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                try
                {
                    ConnectedPeers = new ObservableCollection<Peer>(await DataProvider.GetNodeConnectedPeers());
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            });
        }
    }
}
