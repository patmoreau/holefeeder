import ExpoModulesCore
import UIKit

public class ExpoEndEditing: Module {
  public func definition() -> ModuleDefinition {
    Name("ExpoEndEditing")

    AsyncFunction("endEditing") {
      UIApplication.shared.sendAction(#selector(UIResponder.resignFirstResponder), to: nil, from: nil, for: nil)
    }.runOnQueue(.main)
  }
}
