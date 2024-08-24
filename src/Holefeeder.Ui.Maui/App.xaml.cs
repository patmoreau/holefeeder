namespace Holefeeder.Ui.Maui;

public partial class App
{
    public App()
    {
        InitializeComponent();

        MainPage = new MainPage();
    }

    public static void PerformActionForShortcutItem(string shortcutItemType)
    {
        if (shortcutItemType == "com.drifterapps.Holefeeder.purchase")
        {
            NavigateToPurchase();
        }
    }

    private static void NavigateToPurchase() => Holefeeder.Ui.Maui.MainPage.NavigateToPurchase();
}
