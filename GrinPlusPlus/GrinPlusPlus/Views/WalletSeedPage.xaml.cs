using Xamarin.Forms;

namespace GrinPlusPlus.Views
{
    public partial class WalletSeedPage : ContentPage
    {
        public WalletSeedPage()
        {
            InitializeComponent();
        }

        protected override bool OnBackButtonPressed()
        {
            return false;
        }
    }
}
