using flare_app.ViewModels;

namespace flare_app.Views;

public partial class DiscoveryPage : ContentPage
{
	public DiscoveryPage (MainViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
    }
}