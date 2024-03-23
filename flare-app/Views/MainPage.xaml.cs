using flare_app.ViewModels;
using flare_app.Models;
using Microsoft.Maui.Controls;

namespace flare_app.Views;

public partial class MainPage : ContentPage
{
    private MainViewModel mainViewModel;

    public MainPage(MainViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
        mainViewModel = vm;
    }

    private void search_TextChanged(object sender, TextChangedEventArgs e)
    {
        mainViewModel.PerformMyUserSearchCommand.Execute(e.NewTextValue);
    }
}