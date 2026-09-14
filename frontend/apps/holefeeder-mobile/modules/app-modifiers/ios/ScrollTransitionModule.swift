import ExpoModulesCore
import ExpoUI
import SwiftUI

struct ScrollTransitionModifierView: ViewModifier {
    let scaleIdentity: Double
    let scaleOther: Double
    let opacityIdentity: Double
    let opacityOther: Double

    @ViewBuilder
    func body(content: Content) -> some View {
        if #available(iOS 17.0, tvOS 17.0, *) {
            content.scrollTransition { view, phase in
                view
                .scaleEffect(phase.isIdentity ? scaleIdentity : scaleOther)
                .opacity(phase.isIdentity ? opacityIdentity : opacityOther)
            }
        } else {
            content
        }
    }
}

public class ScrollTransitionModule: Module {
    public func definition() -> ModuleDefinition {
        Name("ScrollTransitionModule")

        OnCreate {
            ViewModifierRegistry.register("scrollTransition") { params, _, _ in
                ScrollTransitionModifierView(
                    scaleIdentity:  params["scaleIdentity"]  as? Double ?? 1.0,
                    scaleOther:     params["scaleOther"]     as? Double ?? 0.85,
                    opacityIdentity: params["opacityIdentity"] as? Double ?? 1.0,
                    opacityOther:   params["opacityOther"]   as? Double ?? 0.6
                )
            }
        }

        OnDestroy {
            ViewModifierRegistry.unregister("scrollTransition")
        }
    }
}