﻿using Xamarin.Forms;

namespace GrinPlusPlus.Views
{
    public partial class OpeningWalletPage : ContentPage
    {
        public OpeningWalletPage()
        {
            InitializeComponent();
        }

        protected override bool OnBackButtonPressed()
        {
            return false;
        }
    }
}
