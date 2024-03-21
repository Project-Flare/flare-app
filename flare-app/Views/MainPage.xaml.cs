using flare_app.ViewModels;

namespace flare_app.Views;

public partial class MainPage : ContentPage
{
    public MainPage(MainViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    private void DiscoveryButton_Tapped(object sender, TappedEventArgs e)
    {
        Shell.Current.GoToAsync(nameof(DiscoveryPage), true);
    }
}