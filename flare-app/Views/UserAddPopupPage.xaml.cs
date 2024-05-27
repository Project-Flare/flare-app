using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.Input;
using flare_app.Services;
using flare_app.ViewModels;
using flare_csharp;
using flare_app;
using SQLite;

namespace flare_app.Views;

public partial class UserAddPopupPage : Popup
{
    private readonly MainViewModel mainViewModel;
    private static bool? smth;

    public UserAddPopupPage(MainViewModel vm)
	{
        InitializeComponent();
        mainViewModel = vm;
    }




    /// <summary>
    /// Closes pop up.
    /// </summary>
    private void Close_Pressed(object sender, EventArgs e)
    {
        Close();
    }

    private async void Add_Pressed(object sender, EventArgs e)
    {
        string text = userToAddEntry.Text;
        bool isuser = await mainViewModel.AddUserOnPop(text);
        if(isuser)
        {
            MauiProgram.ErrorToast("User added");
            Close();
        }
        else
        {
            MauiProgram.ErrorToast("User doesn't exists");
            userToAddEntry.Text = "";
        }
    }
}