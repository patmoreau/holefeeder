import ExpoModulesCore
import SwiftUI

public class HorizontalScrollViewModule: Module {
    public func definition() -> ModuleDefinition {
        Name("HorizontalScrollView")

        View(HorizontalScrollViewSwiftUI.self)
    }
}
