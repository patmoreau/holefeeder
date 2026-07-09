import ExpoModulesCore
import os

public class ExpoOSLogger: Module {
  private let subsystem = Bundle.main.bundleIdentifier ?? "expo.app"

  private func logger(category: String) -> os.Logger {
    os.Logger(subsystem: subsystem, category: category.isEmpty ? "JS" : category)
  }

  public func definition() -> ModuleDefinition {
    Name("ExpoOSLogger")

    Function("log") { (message: String, level: String, category: String) in
      let log = self.logger(category: category)
      switch level.lowercased() {
      case "fault":   log.fault("\(message, privacy: .public)")
      case "error":   log.error("\(message, privacy: .public)")
      case "warn":    log.warning("\(message, privacy: .public)")
      case "info":    log.notice("\(message, privacy: .public)")
      default:        log.debug("\(message, privacy: .public)")
      }
    }
  }
}