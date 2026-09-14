import ExpoModulesCore
import ExpoUI
import SwiftUI

struct ScrollGeometryModifierView: ViewModifier {
    let onOffsetChange: (CGFloat) -> Void

    @ViewBuilder
    func body(content: Content) -> some View {
        if #available(iOS 18.0, tvOS 18.0, *) {
            content
            .onScrollGeometryChange(for: CGFloat.self) { geo in
                geo.contentOffset.y
            } action: { _, newValue in
                onOffsetChange(newValue)
            }
        } else {
            content
        }
    }
}

public class ScrollGeometryModule: Module {
    public func definition() -> ModuleDefinition {
        Name("ScrollGeometryModule")

        OnCreate {
            ViewModifierRegistry.register("onScrollOffsetChange") { params, _, eventDispatcher in
                ScrollGeometryModifierView { offsetY in
                    eventDispatcher(["onScrollOffsetChange": ["offsetY": offsetY]])
                }
            }
        }

        OnDestroy {
            ViewModifierRegistry.unregister("onScrollOffsetChange")
        }
    }
}