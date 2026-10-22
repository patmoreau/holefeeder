import Charts
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
    @Field var identifier: String = ""
}

internal struct IncomeExpenseChartSwiftUI: ExpoSwiftUI.View {
    @ObservedObject var props: IncomeExpenseChartProps

    private var lastIndex: Int { props.points.count - 1 }

    private var yUpperBound: Double {
        let highest = props.points.map { max($0.income, $0.expenses) }.max() ?? 0
        return highest > 0 ? highest * 1.15 : 1
    }

    private func currency(_ value: Double) -> String {
        value.formatted(.currency(code: props.currencyCode).precision(.fractionLength(0)))
    }

    private func endLabel(_ text: String, style: HierarchicalShapeStyle) -> some View {
        Text(text)
            .font(.caption2.weight(.semibold))
            .foregroundStyle(style)
            .padding(.horizontal, 4)
            .padding(.vertical, 1)
            .background(Color(uiColor: .systemBackground).opacity(0.85), in: Capsule())
    }

    var body: some View {
        Chart {
            ForEach(Array(props.points.enumerated()), id: \.offset) { index, point in
                LineMark(
                    x: .value("Period", point.label),
                    y: .value("Amount", point.income),
                    series: .value("Series", props.incomeLabel)
                )
                .foregroundStyle(by: .value("Series", props.incomeLabel))
                .lineStyle(StrokeStyle(lineWidth: 2, lineCap: .round, lineJoin: .round))
                .symbol(Circle())
                .symbolSize(index == lastIndex ? 48 : 16)

                LineMark(
                    x: .value("Period", point.label),
                    y: .value("Amount", point.expenses),
                    series: .value("Series", props.expensesLabel)
                )
                .foregroundStyle(by: .value("Series", props.expensesLabel))
                .lineStyle(StrokeStyle(lineWidth: 2, lineCap: .round, lineJoin: .round, dash: [6, 4]))
                .symbol(Circle())
                .symbolSize(index == lastIndex ? 48 : 16)
            }

            if let last = props.points.last {
                PointMark(x: .value("Period", last.label), y: .value("Amount", last.income))
                    .opacity(0)
                    .annotation(position: .top, alignment: .trailing, spacing: 6) {
                        endLabel(currency(last.income), style: .primary)
                    }
                PointMark(x: .value("Period", last.label), y: .value("Amount", last.expenses))
                    .opacity(0)
                    .annotation(position: .top, alignment: .trailing, spacing: 6) {
                        endLabel(currency(last.expenses), style: .secondary)
                    }
            }
        }
        .chartYScale(domain: 0...yUpperBound)
        .chartForegroundStyleScale([props.incomeLabel: props.incomeColor, props.expensesLabel: props.expensesColor])
        .chartLegend(position: .top, alignment: .leading)
        .chartYAxis {
            AxisMarks(position: .leading) { value in
                AxisGridLine()
                AxisValueLabel {
                    if let amount = value.as(Double.self) {
                        Text(currency(amount))
                    }
                }
            }
        }
        .frame(height: 200)
        .accessibilityElement(children: .ignore)
        .accessibilityLabel(props.accessibilitySummary)
        .accessibilityIdentifier(props.identifier)
    }
}
