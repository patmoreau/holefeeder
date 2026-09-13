import ExpoModulesCore
import UIKit

/// Bridges the scene lifecycle to the module.
///
/// A cold-launch shortcut only reaches the app through its scene's connection
/// options, which arrive after the Expo module registry is built. Holding it
/// here — rather than exposing it as a module constant — lets JavaScript pull it
/// whenever it is ready, so there is no ordering to lose.
public final class QuickActionsStore {
  public static let shared = QuickActionsStore()

  public static let didTriggerAction = Notification.Name("HolefeederQuickActionDidTrigger")

  private var launchAction: UIApplicationShortcutItem?

  private init() {}

  public func recordLaunchAction(_ item: UIApplicationShortcutItem) {
    launchAction = item
  }

  /// Returns the launch action once. Reading clears it so a remount does not navigate again.
  public func takeLaunchAction() -> UIApplicationShortcutItem? {
    defer { launchAction = nil }
    return launchAction
  }

  public func trigger(_ item: UIApplicationShortcutItem) {
    NotificationCenter.default.post(name: Self.didTriggerAction, object: item)
  }
}

internal struct QuickActionRecord: Record {
  @Field var id: String = ""
  @Field var title: String = ""
  @Field var subtitle: String?
  @Field var icon: String?
  @Field var params: [String: String]?
}

internal func serialize(_ item: UIApplicationShortcutItem) -> [String: Any?] {
  [
    "id": item.type,
    "title": item.localizedTitle,
    "subtitle": item.localizedSubtitle,
    "params": item.userInfo as? [String: String]
  ]
}

public class QuickActionsModule: Module {
  public func definition() -> ModuleDefinition {
    Name("QuickActions")

    Events("onQuickAction")

    AsyncFunction("getInitialAction") { () -> [String: Any?]? in
      QuickActionsStore.shared.takeLaunchAction().map(serialize)
    }

    AsyncFunction("setItems") { (items: [QuickActionRecord]) in
      UIApplication.shared.shortcutItems = items.map { item in
        UIApplicationShortcutItem(
          type: item.id,
          localizedTitle: item.title,
          localizedSubtitle: item.subtitle,
          icon: item.icon.map { UIApplicationShortcutIcon(systemImageName: $0) },
          userInfo: item.params?.mapValues { $0 as NSSecureCoding }
        )
      }
    }.runOnQueue(.main)

    OnStartObserving {
      NotificationCenter.default.addObserver(
        self,
        selector: #selector(self.onActionTriggered(_:)),
        name: QuickActionsStore.didTriggerAction,
        object: nil
      )
    }

    OnStopObserving {
      NotificationCenter.default.removeObserver(
        self,
        name: QuickActionsStore.didTriggerAction,
        object: nil
      )
    }
  }

  @objc
  private func onActionTriggered(_ notification: Notification) {
    guard let item = notification.object as? UIApplicationShortcutItem else { return }
    sendEvent("onQuickAction", serialize(item))
  }
}
