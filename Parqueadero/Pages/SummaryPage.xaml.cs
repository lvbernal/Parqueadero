using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace Parqueadero.Pages
{
    public partial class SummaryPage : ContentPage
    {
        public SummaryPage()
        {
            InitializeComponent();
        }

        public void OnItemTapped(object sender, ItemTappedEventArgs e)
        {
            ((ListView)sender).SelectedItem = null;
        }
    }
}
