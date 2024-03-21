using flare_app.ViewModels;
using flare_csharp;

namespace flare_app.Views;

public partial class DiscoveryPage : ContentPage
{
	public DiscoveryPage (MainViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;

    }

	private async void TF()
	{
		await Client.FillUserDiscovery();
	}
}