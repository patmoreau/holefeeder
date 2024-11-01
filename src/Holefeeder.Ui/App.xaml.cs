namespace Holefeeder.Ui;

public partial class App : Application
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

    private static void NavigateToPurchase() => Ui.MainPage.NavigateToPurchase();
}
