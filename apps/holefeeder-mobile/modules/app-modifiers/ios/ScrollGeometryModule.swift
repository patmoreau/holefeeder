import ExpoModulesCore
import ExpoUI
import SwiftUI

struct ScrollGeometryModifierView: ViewModifier {
    let onOffsetChange: (CGFloat) -> Void

    func body(content: Content) -> some View {
        content
        .onScrollGeometryChange(for: CGFloat.self) { geo in
            geo.contentOffset.y
        } action: { _, newValue in
            onOffsetChange(newValue)
        }
    }
}

public class ScrollGeometryModule: Module {
    public func definition() -> ModuleDefinition {
        Name("ScrollGeometryModule")

        OnCreate {
            ViewModifierRegistry.register("onScrollOffsetChange") { params, _, eventDispatcher in
                ScrollGeometryModifierView { offsetY in
                    eventDispatcher?.dispatch("onScrollOffsetChange", payload: ["offsetY": offsetY])
                }
            }
        }

        OnDestroy {
            ViewModifierRegistry.unregister("onScrollOffsetChange")
        }
    }
}