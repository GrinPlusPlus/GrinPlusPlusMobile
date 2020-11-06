using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;

namespace GrinPlusPlus.Views
{
    public partial class DashboardCarouselPage : Xamarin.Forms.TabbedPage
    {
        public DashboardCarouselPage()
        {
            InitializeComponent();

            CurrentPage = Children[1];
        }
    }
}
