namespace Holefeeder.Ui;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    public static void NavigateToPurchase()
    {
        var mainComponent = Components.Main.Instance;
        mainComponent?.NavigateToPurchase();
    }
}
