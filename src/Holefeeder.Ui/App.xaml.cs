using System.Globalization;

namespace Holefeeder.Ui;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        ExceptionLogger.Initialize(); // Initialize logging
        MauiExceptions.UnhandledException += (sender, args) =>
        {
            ExceptionLogger.LogException((Exception)args.ExceptionObject, "Unhandled AppDomain Exception");
        };
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new MainPage()) { Title = "Holefeeder" };
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
