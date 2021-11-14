using Xamarin.Forms;

namespace GrinPlusPlus.Views
{
    public partial class WalletLoginPage : ContentPage
    {
        public WalletLoginPage()
        {
            InitializeComponent();
        }

        protected override bool OnBackButtonPressed()
        {
            return false;
        }
    }
}
