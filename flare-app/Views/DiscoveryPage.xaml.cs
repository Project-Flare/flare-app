using flare_app.Services;
using flare_app.ViewModels;
using flare_csharp;

namespace flare_app.Views;

public partial class DiscoveryPage : ContentPage
{
    private readonly MainViewModel _mainViewModel;

    public DiscoveryPage (MainViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
        _mainViewModel = vm;
    }

    private void search_TextChanged(object sender, TextChangedEventArgs e)
    {
        _mainViewModel.PerformDiscoverySearchCommand.Execute(e.NewTextValue);
    }
}