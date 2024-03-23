using flare_app.ViewModels;
using flare_csharp;

namespace flare_app.Views;

public partial class DiscoveryPage : ContentPage
{
    private MainViewModel mainViewModel;

    public DiscoveryPage (MainViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
        mainViewModel = vm;

    }

    private void search_TextChanged(object sender, TextChangedEventArgs e)
    {
        mainViewModel.PerformDiscoverySearchCommand.Execute(e.NewTextValue);
    }
}