//
//  ContentView.swift
//  Holefeeder
//
//  Created by Patrick Moreau on 2024-04-25.
//

import SwiftUI

struct ContentView: View {
    var buildConfiguration: String {
        #if DEBUG
        return "Debug"
        #elseif STAGING
        return "Staging"
        #elseif RELEASE
        return "Release"
        #else
        return "Unknown"
        #endif
    }

    var body: some View {
        VStack {
            Image(systemName: "globe")
                .imageScale(.large)
                .foregroundStyle(.tint)
            Text("Hello, world! - Build Configuration: \(buildConfiguration)")
        }
        .padding()
    }
}

#Preview {
    ContentView()
}
