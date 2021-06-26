using Xamarin.Forms;

namespace GrinPlusPlus.Views
{
    public partial class SendGrinsUsingQRPage : ContentPage
    {
        public SendGrinsUsingQRPage()
        {
            InitializeComponent();
        }

        protected override bool OnBackButtonPressed()
        {
            return false;
        }
    }
}
