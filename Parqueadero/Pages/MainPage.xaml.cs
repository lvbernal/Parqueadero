using Parqueadero.ViewModels;
using Xamarin.Forms;

namespace Parqueadero.Pages
{
    public partial class MainPage : ContentPage
    {
        private MainViewModel context;

        public MainPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
        }
    }
}
