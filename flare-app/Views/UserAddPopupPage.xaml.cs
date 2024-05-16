using CommunityToolkit.Maui.Views;
using flare_app.ViewModels;

namespace flare_app.Views;

public partial class UserAddPopupPage : Popup
{
    private readonly MainViewModel mainViewModel;

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

    private void Add_Pressed(object sender, EventArgs e)
    {
        string text = userToAddEntry.Text;
        mainViewModel.AddUserOnPopCommand.Execute(text);
        // Late
        userToAddEntry.Text = "";
        Close();
    }
}