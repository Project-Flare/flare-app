using flare_app.ViewModels;
using flare_app.Models;
using Microsoft.Maui.Controls;
using flare_app.Services;
using CommunityToolkit.Maui.Views;
using flare_app.Views.Templates;
using flare_csharp;

namespace flare_app.Views;

public partial class MainPage : ContentPage
{
    private readonly MainViewModel _mainViewModel;

    public MainPage(MainViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
        _mainViewModel = vm;
        if (!MessagingService.Instance.IsRunning)
            MessagingService.Instance.StartServices();
    }

    /// <summary>
    /// Performs search in my contact list with relay command.
    /// </summary>
    private void search_TextChanged(object sender, TextChangedEventArgs e)
    {
        _mainViewModel.PerformMyUserSearchCommand.Execute(e.NewTextValue);
    }

    /// <summary>
    /// Calls 'add user' pop up.
    /// </summary>
    private void Button_Clicked(object sender, TappedEventArgs e)
    {
        this.ShowPopup(new UserAddPopupPage(_mainViewModel));
    }

    /// <summary>
    /// Navigates to settings page.
    /// </summary>
    private async void Settings_Tapped(object sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(SettingsPage));
    }
}