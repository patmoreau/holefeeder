namespace Holefeeder.Ui.Maui;

public partial class MainPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    public static void NavigateToPurchase()
    {
        var mainComponent = Main.Instance;
        mainComponent?.NavigateToPurchase();
    }
}
