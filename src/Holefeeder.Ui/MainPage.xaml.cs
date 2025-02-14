using Holefeeder.Ui.Shared;

namespace Holefeeder.Ui;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    public static void NavigateToPurchase()
    {
        var mainComponent = Routes.Instance;
        mainComponent?.NavigateToPurchase();
    }
}
