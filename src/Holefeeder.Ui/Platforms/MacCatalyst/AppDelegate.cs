﻿using Foundation;
using UIKit;

namespace Holefeeder.Ui;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

    public override void PerformActionForShortcutItem(UIApplication application, UIApplicationShortcutItem shortcutItem, UIOperationHandler completionHandler)
    {
        ArgumentNullException.ThrowIfNull(shortcutItem);
        ArgumentNullException.ThrowIfNull(completionHandler);

        // ReSharper disable once SuspiciousTypeConversion.Global
        App.PerformActionForShortcutItem(shortcutItem.Type);

        completionHandler(true);
    }
}
