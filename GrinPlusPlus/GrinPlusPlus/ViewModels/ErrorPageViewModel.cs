using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Essentials;

namespace GrinPlusPlus.ViewModels
{
    public class ErrorPageViewModel : BindableBase
    {
        public DelegateCommand SupportButtonClickedCommand => new DelegateCommand(SupportButtonClicked);

        async void SupportButtonClicked()
        {
            await Launcher.TryOpenAsync(new Uri("https://t.me/GrinPP"));
        }

        public ErrorPageViewModel()
        {

        }
    }
}
