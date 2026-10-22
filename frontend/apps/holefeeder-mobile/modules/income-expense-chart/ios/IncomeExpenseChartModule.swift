import ExpoModulesCore
import SwiftUI

public class IncomeExpenseChartModule: Module {
    public func definition() -> ModuleDefinition {
        Name("IncomeExpenseChart")

        View(IncomeExpenseChartSwiftUI.self)
    }
}
