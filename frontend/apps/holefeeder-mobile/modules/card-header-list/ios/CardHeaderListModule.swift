import ExpoModulesCore
import SwiftUI

public class CardHeaderListModule: Module {
    public func definition() -> ModuleDefinition {
        Name("CardHeaderList")

        View(CardHeaderListSwiftUI.self)
    }
}
