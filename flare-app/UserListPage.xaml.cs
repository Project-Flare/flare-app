using Flare;
using Microsoft.VisualBasic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;

namespace flare_app
{
    public partial class UserListPage : ContentPage
    {
        public UserListPage()
        {
            InitializeComponent();

            List<string> data = new List<string>();
            data.Add("#D9D9D9");
            data.Add("#8A8888");
            data.Add("#D9D9D9");

            UserListCollection.ItemsSource = data;
        }

        private void DiscoveryButton_Tapped(object sender, TappedEventArgs e)
        {
            Shell.Current.GoToAsync(nameof(DiscoveryPage));
        }
    }
}
