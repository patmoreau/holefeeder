import ExpoModulesCore
import SwiftUI

internal struct IncomeExpenseChartPoint: Record {
    @Field var label: String = ""
    @Field var income: Double = 0
    @Field var expenses: Double = 0
}

internal final class IncomeExpenseChartProps: ExpoSwiftUI.ViewProps {
    @Field var points: [IncomeExpenseChartPoint] = []
    @Field var incomeLabel: String = ""
    @Field var expensesLabel: String = ""
    @Field var incomeColor: Color = .green
    @Field var expensesColor: Color = .orange
    @Field var currencyCode: String = "CAD"
    @Field var accessibilitySummary: String = ""
}

internal struct IncomeExpenseChartSwiftUI: ExpoSwiftUI.View {
    @ObservedObject var props: IncomeExpenseChartProps

    var body: some View {
        EmptyView()
    }
}
