import ExpoModulesCore
import SwiftUI

internal final class CardHeaderListProps: ExpoSwiftUI.ViewProps {
    @Field var contentInsetTop: Double = 300
    var onScrollOffsetChange = EventDispatcher()
}

private struct ScrollOffsetKey: PreferenceKey {
    static var defaultValue: CGFloat = 0
    static func reduce(value: inout CGFloat, nextValue: () -> CGFloat) {
        value = nextValue()
    }
}

internal struct CardHeaderListSwiftUI: ExpoSwiftUI.View {
    @ObservedObject var props: CardHeaderListProps

    var body: some View {
        List {
            Color.clear
                .frame(height: CGFloat(props.contentInsetTop))
                .listRowSeparator(.hidden)
                .listRowInsets(EdgeInsets())
                .background(
                    GeometryReader { geo in
                        Color.clear.preference(
                            key: ScrollOffsetKey.self,
                            value: geo.frame(in: .named("cardHeaderList")).minY
                        )
                    }
                )
            Children()
        }
        .coordinateSpace(name: "cardHeaderList")
        .listStyle(.plain)
        .scrollContentBackground(.hidden)
        .onPreferenceChange(ScrollOffsetKey.self) { minY in
            let offset = max(0.0, -minY)
            props.onScrollOffsetChange(["offset": Double(offset)])
        }
    }
}
