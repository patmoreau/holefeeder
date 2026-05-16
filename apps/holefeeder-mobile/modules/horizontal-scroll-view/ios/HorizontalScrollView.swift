import ExpoModulesCore
import SwiftUI

internal final class HorizontalScrollViewProps: ExpoSwiftUI.ViewProps {
}

internal struct HorizontalScrollViewSwiftUI: ExpoSwiftUI.View {
    @ObservedObject var props: HorizontalScrollViewProps

    var body: some View {
        ScrollView(.horizontal, showsIndicators: false) {
            HStack(spacing: 0) {
                Children()
            }
        }
    }
}
