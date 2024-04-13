using flare_app.ViewModels;
using flare_app.Models;
using Microsoft.Maui.Controls;
using flare_app.Services;
using CommunityToolkit.Maui.Views;

namespace flare_app.Views;

public partial class MainPage : ContentPage
{
    private readonly MainViewModel _mainViewModel;
    private bool firstRefresh = false;

    public MainPage(MainViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
        _mainViewModel = vm;
    }

    private void search_TextChanged(object sender, TextChangedEventArgs e)
    {
        _mainViewModel.PerformMyUserSearchCommand.Execute(e.NewTextValue);
    }

    private void Button_Clicked(object sender, EventArgs e)
    {
        this.ShowPopup(new UserAddPopupPage());
    }

    private async void Settings_Tapped(object sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(SettingsPage));
    }
}