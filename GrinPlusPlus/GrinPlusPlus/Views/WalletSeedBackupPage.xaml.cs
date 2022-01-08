using Xamarin.Forms;

namespace GrinPlusPlus.Views
{
    public partial class WalletSeedBackupPage : ContentPage
    {
        public WalletSeedBackupPage()
        {
            InitializeComponent();
        }

        protected override bool OnBackButtonPressed()
        {
            return false;
        }
    }
}
