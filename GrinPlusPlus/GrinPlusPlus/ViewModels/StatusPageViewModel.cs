using GrinPlusPlus.Api;
using GrinPlusPlus.Models;
using Prism.Navigation;
using Prism.Services;
using Prism.Services.Dialogs;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace GrinPlusPlus.ViewModels
{
    public class StatusPageViewModel : ViewModelBase
    {
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


        public StatusPageViewModel(INavigationService navigationService, IDataProvider dataProvider, IDialogService dialogService, IPageDialogService pageDialogService)
            : base(navigationService, dataProvider, dialogService, pageDialogService)
        {
            UpdateStatus();
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await ListConnectedPeersAsync();
            });
        }
        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            Device.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
                if (Settings.IsLoggedIn == false)
                {
                    return false;
                }

                UpdateStatus();

                return true;
            });

            Device.StartTimer(TimeSpan.FromSeconds(10), () =>
            {
                if (Settings.IsLoggedIn == false)
                {
                    return false;
                }

                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await ListConnectedPeersAsync();
                });

                return true;
            });
        }

        private void UpdateStatus()
        {
            if (!Status.Equals(Settings.Node.Status))
            {
                Status = Settings.Node.Status;
            }
            if (!HeaderHeight.Equals(Settings.Node.HeaderHeight))
            {
                HeaderHeight = Settings.Node.HeaderHeight;
            }
            if (!Blocks.Equals(Settings.Node.Blocks))
            {
                Blocks = Settings.Node.Blocks;
            }
            if (!NetworkHeight.Equals(Settings.Node.NetworkHeight))
            {
                NetworkHeight = Settings.Node.NetworkHeight;
            }
        }

        private async Task ListConnectedPeersAsync()
        {

            try
            {
                var connectedPeers = await DataProvider.GetNodeConnectedPeers();
                if (ConnectedPeers.Count > 0)
                {
                    foreach (Peer peer in ConnectedPeers)
                    {
                        if (!connectedPeers.Any(p => p.Address.Equals(peer.Address)))
                        {
                            ConnectedPeers.Remove(peer);
                        }
                    }
                }

                foreach (Peer peer in connectedPeers)
                {
                    if (!ConnectedPeers.Any(p => p.Address.Equals(peer.Address)))
                    {
                        ConnectedPeers.Add(peer);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

        }
    }
}
