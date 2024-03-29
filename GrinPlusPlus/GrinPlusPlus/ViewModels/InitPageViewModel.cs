﻿using GrinPlusPlus.Api;
using GrinPlusPlus.Models;
using GrinPlusPlus.Resources;
using Prism.Navigation;
using Prism.Services;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace GrinPlusPlus.ViewModels
{
    public class InitPageViewModel : ViewModelBase
    {
        private string status = "Initializing Node...";
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

        private string _progressPercentage = string.Empty;
        public string ProgressPercentage
        {
            get { return _progressPercentage; }
            set { SetProperty(ref _progressPercentage, value); }
        }

        private ObservableCollection<Fact> _facts = new ObservableCollection<Fact>();
        public ObservableCollection<Fact> Facts
        {
            get { return _facts; }
            set { SetProperty(ref _facts, value); }
        }

        private bool StopTimer = false;

        public InitPageViewModel(INavigationService navigationService, IDataProvider dataProvider, IDialogService dialogService, IPageDialogService pageDialogService) 
            : base(navigationService, dataProvider, dialogService, pageDialogService)
        {
            Facts.Add(new Fact { Title = AppResources.ResourceManager.GetString("Mimblewimble"), Details = AppResources.ResourceManager.GetString("FactsMimblewimble") });
            Facts.Add(new Fact { Title = AppResources.ResourceManager.GetString("Addresses"), Details = AppResources.ResourceManager.GetString("FactAddresses") });
            Facts.Add(new Fact { Title = AppResources.ResourceManager.GetString("Amounts"), Details = AppResources.ResourceManager.GetString("FactsAmounts") });
            Facts.Add(new Fact { Title = AppResources.ResourceManager.GetString("Emission"), Details = AppResources.ResourceManager.GetString("FactEmission") });
            Facts.Add(new Fact { Title = AppResources.ResourceManager.GetString("Slatepack"), Details = AppResources.ResourceManager.GetString("FactSlatepack") });
            Facts.Add(new Fact { Title = AppResources.ResourceManager.GetString("Transactions"), Details = AppResources.ResourceManager.GetString("FactTransactions") });
            Facts.Add(new Fact { Title = AppResources.ResourceManager.GetString("Private"), Details = AppResources.ResourceManager.GetString("FactPrivate") });
            Facts.Add(new Fact { Title = AppResources.ResourceManager.GetString("Scalable"), Details = AppResources.ResourceManager.GetString("FactsScalable") });
            Facts.Add(new Fact { Title = AppResources.ResourceManager.GetString("Open"), Details = AppResources.ResourceManager.GetString("FactsOpen") });
            Facts.Add(new Fact { Title = AppResources.ResourceManager.GetString("Dandelion"), Details = AppResources.ResourceManager.GetString("FactsDandelion") });

            Device.StartTimer(TimeSpan.FromSeconds(5), () =>
            {
                if (!Settings.IsNodeRunning)
                {
                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        await NavigationService.NavigateAsync("/NavigationPage/ErrorPage");
                    });
                }

                return !StopTimer;
            });

            Device.StartTimer(TimeSpan.FromSeconds(2), () =>
            {
                Status = Settings.Node.Status.Equals("Not Running") && Settings.IsNodeRunning ? "Initializing Node..." : Settings.Node.Status;
                ProgressBarr = Settings.Node.ProgressPercentage;
                ProgressPercentage = string.Format($"{ Settings.Node.ProgressPercentage * 100:F}");

                return !StopTimer;
            });

            Device.StartTimer(TimeSpan.FromSeconds(5), () =>
            {
                if (StopTimer)
                {
                    return false;
                }

                if (Settings.Node.Status.Equals("Not Running") || Settings.Node.Status.Equals("Waiting for Peers"))
                {
                    return true;
                }

                ContinueNextStep();

                return !StopTimer;
            });
        }

        private void ContinueNextStep()
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                try
                {
                    List<Account> accounts = await DataProvider.GetAccounts();

                    StopTimer = true;

                    await Task.Delay(TimeSpan.FromSeconds(2));

                    if (accounts.Count == 0)
                    {
                        await NavigationService.NavigateAsync("/NavigationPage/LoginPage");
                    }
                    else if (accounts.Count == 1)
                    {
                        await NavigationService.NavigateAsync("/NavigationPage/WalletLoginPage", new NavigationParameters { { "username", accounts[0].Name } });
                    }
                    else
                    {
                        IActionSheetButton[] buttons = new ActionSheetButton[accounts.Count];
                        for (int i = 0; i < accounts.Count; i++)
                        {
                            string account = accounts[i].Name;
                            buttons[i] = ActionSheetButton.CreateButton(account, async () =>
                            {
                                await NavigationService.NavigateAsync("/NavigationPage/WalletLoginPage", new NavigationParameters { { "username", account } });
                            });
                        }
                        await PageDialogService.DisplayActionSheetAsync(string.Empty, buttons);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);

                }
            });
        }

        public override void OnNavigatedFrom(INavigationParameters parameters)
        {
            StopTimer = true;
        }
    }
}
