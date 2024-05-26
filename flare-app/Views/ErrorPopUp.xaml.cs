using CommunityToolkit.Maui.Views;

namespace flare_app.Views;

public partial class ErrorPopUp : Popup
{
    public ErrorPopUp(string header, string message, string response)
    {
        InitializeComponent();
        Header.Text = header;
        ErrorMessage.Text = message;
        ResponseButton.Text = response;
    }

    private void Button_Pressed(object sender, EventArgs e)
    {
        Close();
    }
}