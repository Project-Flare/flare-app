using flare_app.ViewModel;

namespace flare_app.Views;

public partial class DiscoveryPage : ContentPage
{
	public DiscoveryPage (MainViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
    }
}