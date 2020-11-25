using GrinPlusPlus.Api;
using GrinPlusPlus.Models;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using Prism.Services.Dialogs;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace GrinPlusPlus.ViewModels
{
    public class StatusPageViewModel : ViewModelBase
    {
        private NodeStatus status;
        public NodeStatus Status
        {
            get { return status; }
            set { SetProperty(ref status, value); }
        }

        private ObservableCollection<Peer> _connectedPeers;
        public ObservableCollection<Peer> ConnectedPeers
        {
            get { return _connectedPeers; }
            set { SetProperty(ref _connectedPeers, value); }
        }
        public DelegateCommand LogoutCommand => new DelegateCommand(Logout);

        async void Logout()
        {
            await DataProvider.DoLogout(await SecureStorage.GetAsync("token"));
            Settings.IsLoggedIn = false;
            await NavigationService.NavigateAsync("/SharedTransitionNavigationPage/LoginPage");
        }

        public StatusPageViewModel(INavigationService navigationService, IDataProvider dataProvider, IDialogService dialogService, IPageDialogService pageDialogService)
            : base(navigationService, dataProvider, dialogService, pageDialogService)
        {
            ConnectedPeers = new ObservableCollection<Peer>();

            Device.StartTimer(TimeSpan.FromSeconds(3), () =>
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    try
                    {
                        var status = await DataProvider.GetNodeStatus();
                        if (Status == null)
                        {
                            Status = status;
                            return;
                        }
                        if (!status.Blocks.Equals(Status.Blocks) || !status.Headers.Equals(Status.Headers) || !status.Network.Equals(Status.Network))
                        {
                            Status = status;
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                });
                return Settings.IsLoggedIn;
            });

            Device.StartTimer(TimeSpan.FromSeconds(5), () =>
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    try
                    {
                        var connectedPeers = await DataProvider.GetNodeConnectedPeers();
                        
                        if(connectedPeers.Count>0)
                        {
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
                        
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                });
                return Settings.IsLoggedIn;
            });
        }
    }
}
