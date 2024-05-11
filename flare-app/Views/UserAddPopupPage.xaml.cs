using CommunityToolkit.Maui.Views;
using flare_app.ViewModels;

namespace flare_app.Views;

public partial class UserAddPopupPage : Popup
{
    public UserAddPopupPage()
	{
        InitializeComponent();
	}

    /// <summary>
    /// Closes pop up.
    /// </summary>
    private void Button_Pressed(object sender, EventArgs e)
    {
        Close();
    }
}