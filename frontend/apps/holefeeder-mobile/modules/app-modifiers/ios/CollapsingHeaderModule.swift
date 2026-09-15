import ExpoModulesCore
import ExpoUI
import SwiftUI

final class CollapsingHeaderStore: ObservableObject {
    @Published var offsetY: CGFloat = 0
}

final class CollapsingHeaderRegistry {
    static let shared = CollapsingHeaderRegistry()

    private var stores: [String: CollapsingHeaderStore] = [:]

    func store(for id: String) -> CollapsingHeaderStore {
        if let existing = stores[id] {
            return existing
        }
        let store = CollapsingHeaderStore()
        stores[id] = store
        return store
    }

    func discard(id: String) {
        stores.removeValue(forKey: id)
    }
}

private func interpolate(
    _ value: CGFloat,
    inputMin: CGFloat,
    inputMax: CGFloat,
    outputMin: CGFloat,
    outputMax: CGFloat
) -> CGFloat {
    guard inputMax > inputMin else { return outputMin }
    let progress = min(max((value - inputMin) / (inputMax - inputMin), 0), 1)
    return outputMin + progress * (outputMax - outputMin)
}

// JavaScript numbers reach the registry as a Swift `Double`; the other cases are there so a
// dimension can never silently read as zero, which collapses the header to nothing.
private func length(_ params: [String: Any], _ key: String, fallback: CGFloat = 0) -> CGFloat {
    if let double = params[key] as? Double {
        return CGFloat(double)
    }
    if let integer = params[key] as? Int {
        return CGFloat(integer)
    }
    if let number = params[key] as? NSNumber {
        return CGFloat(number.doubleValue)
    }
    return fallback
}

private func color(fromHex hex: String?) -> Color {
    guard let hex else { return .clear }
    var digits = hex.trimmingCharacters(in: .whitespacesAndNewlines)
    if digits.hasPrefix("#") {
        digits.removeFirst()
    }
    guard digits.count == 6 || digits.count == 8, let value = UInt64(digits, radix: 16) else {
        return .clear
    }
    let hasAlpha = digits.count == 8
    let red = Double((value >> (hasAlpha ? 24 : 16)) & 0xFF) / 255
    let green = Double((value >> (hasAlpha ? 16 : 8)) & 0xFF) / 255
    let blue = Double((value >> (hasAlpha ? 8 : 0)) & 0xFF) / 255
    let alpha = hasAlpha ? Double(value & 0xFF) / 255 : 1
    return Color(.sRGB, red: red, green: green, blue: blue, opacity: alpha)
}

/// Feeds the scroll offset straight into the store on the main thread, so the header never
/// waits on a round trip through JavaScript to follow the list.
struct CollapsingHeaderSourceModifierView: ViewModifier {
    let store: CollapsingHeaderStore

    @ViewBuilder
    func body(content: Content) -> some View {
        if #available(iOS 18.0, tvOS 18.0, *) {
            content
            .onScrollGeometryChange(for: CGFloat.self) { geometry in
                geometry.contentOffset.y
            } action: { _, newValue in
                store.offsetY = newValue
            }
        } else {
            content
        }
    }
}

// A React Native view hosted inside SwiftUI keeps the size React gave it and ignores SwiftUI's
// size proposal, so nothing here may rely on a frame to lay the cards out — only on the visual
// modifiers that do carry across the boundary, `opacity` and `offset`. The collapsing bar is a
// plain SwiftUI shape precisely because it is the one part that does have to resize.
private func barHeight(_ offsetY: CGFloat, _ fullHeight: CGFloat, _ collapsedHeight: CGFloat) -> CGFloat {
    interpolate(
        offsetY,
        inputMin: 0,
        inputMax: max(fullHeight - collapsedHeight, 1),
        outputMin: fullHeight,
        outputMax: collapsedHeight
    )
}

struct CollapsingHeaderBarModifierView: ViewModifier {
    @ObservedObject var store: CollapsingHeaderStore
    let fullHeight: CGFloat
    let collapsedHeight: CGFloat
    let backgroundColor: Color

    func body(content: Content) -> some View {
        content
        .foregroundStyle(backgroundColor)
        .frame(
            maxWidth: .infinity,
            maxHeight: barHeight(store.offsetY, fullHeight, collapsedHeight),
            alignment: .top
        )
    }
}

struct CollapsingHeaderLargeCardModifierView: ViewModifier {
    @ObservedObject var store: CollapsingHeaderStore
    let travel: CGFloat
    let fadeEnd: CGFloat

    func body(content: Content) -> some View {
        content
        .opacity(
            interpolate(store.offsetY, inputMin: 0, inputMax: travel * fadeEnd, outputMin: 1, outputMax: 0)
        )
        .offset(
            y: -interpolate(store.offsetY, inputMin: 0, inputMax: travel, outputMin: 0, outputMax: travel)
        )
    }
}

struct CollapsingHeaderSmallCardModifierView: ViewModifier {
    @ObservedObject var store: CollapsingHeaderStore
    let fullHeight: CGFloat
    let collapsedHeight: CGFloat
    let rowInset: CGFloat
    let rowHeight: CGFloat
    let fadeStart: CGFloat
    let fadeEnd: CGFloat

    func body(content: Content) -> some View {
        let travel = max(fullHeight - collapsedHeight, 1)

        // Rides the bar's bottom edge down, since anchoring to it is not available across the
        // hosting boundary.
        return content
        .opacity(
            interpolate(
                store.offsetY,
                inputMin: travel * fadeStart,
                inputMax: travel * fadeEnd,
                outputMin: 0,
                outputMax: 1
            )
        )
        .offset(y: barHeight(store.offsetY, fullHeight, collapsedHeight) - rowInset - rowHeight)
        .allowsHitTesting(false)
    }
}

public class CollapsingHeaderModule: Module {
    public func definition() -> ModuleDefinition {
        Name("CollapsingHeaderModule")

        OnCreate {
            ViewModifierRegistry.register("collapsingHeaderSource") { params, _, _ in
                CollapsingHeaderSourceModifierView(
                    store: CollapsingHeaderRegistry.shared.store(for: params["id"] as? String ?? "")
                )
            }

            ViewModifierRegistry.register("collapsingHeaderBar") { params, _, _ in
                CollapsingHeaderBarModifierView(
                    store: CollapsingHeaderRegistry.shared.store(for: params["id"] as? String ?? ""),
                    fullHeight: length(params, "fullHeight"),
                    collapsedHeight: length(params, "collapsedHeight"),
                    backgroundColor: color(fromHex: params["backgroundColor"] as? String)
                )
            }

            ViewModifierRegistry.register("collapsingHeaderLargeCard") { params, _, _ in
                CollapsingHeaderLargeCardModifierView(
                    store: CollapsingHeaderRegistry.shared.store(for: params["id"] as? String ?? ""),
                    travel: length(params, "travel"),
                    fadeEnd: length(params, "fadeEnd", fallback: 1)
                )
            }

            ViewModifierRegistry.register("collapsingHeaderSmallCard") { params, _, _ in
                CollapsingHeaderSmallCardModifierView(
                    store: CollapsingHeaderRegistry.shared.store(for: params["id"] as? String ?? ""),
                    fullHeight: length(params, "fullHeight"),
                    collapsedHeight: length(params, "collapsedHeight"),
                    rowInset: length(params, "rowInset"),
                    rowHeight: length(params, "rowHeight"),
                    fadeStart: length(params, "fadeStart"),
                    fadeEnd: length(params, "fadeEnd", fallback: 1)
                )
            }
        }

        OnDestroy {
            ViewModifierRegistry.unregister("collapsingHeaderSource")
            ViewModifierRegistry.unregister("collapsingHeaderBar")
            ViewModifierRegistry.unregister("collapsingHeaderLargeCard")
            ViewModifierRegistry.unregister("collapsingHeaderSmallCard")
        }
    }
}
