using flare_app.ViewModel;

namespace flare_app;

public partial class DiscoveryPage : ContentPage
{
	public DiscoveryPage(MainViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}