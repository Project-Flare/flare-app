using Flare;
using flare_app.ViewModel;
using Microsoft.VisualBasic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;

namespace flare_app
{
    public partial class UserListPage : ContentPage
    {
        public UserListPage(MainViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }

        private void DiscoveryButton_Tapped(object sender, TappedEventArgs e)
        {
            Shell.Current.GoToAsync(nameof(DiscoveryPage));
        }
    }
}
