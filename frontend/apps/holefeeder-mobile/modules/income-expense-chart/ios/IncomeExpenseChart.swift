import Charts
import ExpoModulesCore
import SwiftUI

internal struct IncomeExpenseChartPoint: Record {
    @Field var label: String = ""
    @Field var income: Double = 0
    @Field var expenses: Double = 0
    @Field var tooltipTitle: String = ""
    @Field var netText: String = ""
    @Field var netTone: String = "neutral"
}

internal struct IncomeExpenseChartBandPoint: Record {
    @Field var x: Double = 0
    @Field var low: Double = 0
    @Field var high: Double = 0
}

internal struct IncomeExpenseChartBand: Record {
    @Field var kind: String = "surplus"
    @Field var points: [IncomeExpenseChartBandPoint] = []
}

internal final class IncomeExpenseChartProps: ExpoSwiftUI.ViewProps {
    @Field var points: [IncomeExpenseChartPoint] = []
    @Field var incomeLabel: String = ""
    @Field var expensesLabel: String = ""
    @Field var incomeColor: Color = .green
    @Field var expensesColor: Color = .orange
    @Field var bands: [IncomeExpenseChartBand] = []
    @Field var deficitLabel: String = ""
    @Field var deficitColor: Color = .red
    @Field var positiveColor: Color = .green
    @Field var negativeColor: Color = .red
    @Field var currencyCode: String = "CAD"
    @Field var accessibilitySummary: String = ""
    @Field var identifier: String = ""
}

internal struct IncomeExpenseChartSwiftUI: ExpoSwiftUI.View {
    @ObservedObject var props: IncomeExpenseChartProps
    @State private var selectedIndex: Int?

    private let tooltipWidth: CGFloat = 150

    private var lastIndex: Int { props.points.count - 1 }

    private var yUpperBound: Double {
        let highest = props.points.map { max($0.income, $0.expenses) }.max() ?? 0
        return highest > 0 ? highest * 1.15 : 1
    }

    private func currency(_ value: Double) -> String {
        value.formatted(.currency(code: props.currencyCode).precision(.fractionLength(0)))
    }

    private func amount(_ value: Double) -> String {
        value.formatted(.currency(code: props.currencyCode))
    }

    private func netColor(_ tone: String) -> Color {
        switch tone {
        case "positive": return props.positiveColor
        case "negative": return props.negativeColor
        default: return .primary
        }
    }

    private func select(atPlotX x: CGFloat, proxy: ChartProxy) {
        guard let position: Double = proxy.value(atX: x), !props.points.isEmpty else { return }
        let index = min(max(Int(position.rounded()), 0), lastIndex)
        if index != selectedIndex {
            selectedIndex = index
            UISelectionFeedbackGenerator().selectionChanged()
        }
    }

    private func legendLine(_ color: Color, _ label: String, _ value: Double) -> some View {
        HStack(spacing: 4) {
            Circle().fill(color).frame(width: 6, height: 6)
            Text(label).foregroundStyle(.secondary)
            Spacer(minLength: 4)
            Text(amount(value)).foregroundStyle(.primary)
        }
        .font(.caption2)
    }

    private func tooltip(for point: IncomeExpenseChartPoint) -> some View {
        VStack(alignment: .leading, spacing: 3) {
            Text(point.tooltipTitle).font(.caption.weight(.semibold))
            legendLine(props.incomeColor, props.incomeLabel, point.income)
            legendLine(props.expensesColor, props.expensesLabel, point.expenses)
            Text(point.netText).font(.caption2.weight(.semibold)).foregroundStyle(netColor(point.netTone))
        }
        .padding(8)
        .frame(width: tooltipWidth, alignment: .leading)
        .background(.regularMaterial, in: RoundedRectangle(cornerRadius: 10))
        .shadow(color: .black.opacity(0.12), radius: 6, y: 2)
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
            ForEach(Array(props.bands.enumerated()), id: \.offset) { bandIndex, band in
                ForEach(Array(band.points.enumerated()), id: \.offset) { _, point in
                    if band.kind == "deficit" {
                        AreaMark(
                            x: .value("Period", point.x),
                            yStart: .value("Low", point.low),
                            yEnd: .value("High", point.high),
                            series: .value("Band", bandIndex)
                        )
                        .foregroundStyle(by: .value("Series", props.deficitLabel))
                    } else {
                        AreaMark(
                            x: .value("Period", point.x),
                            yStart: .value("Low", point.low),
                            yEnd: .value("High", point.high),
                            series: .value("Band", bandIndex)
                        )
                        .foregroundStyle(props.incomeColor.opacity(0.15))
                    }
                }
            }

            ForEach(Array(props.points.enumerated()), id: \.offset) { index, point in
                LineMark(
                    x: .value("Period", Double(index)),
                    y: .value("Amount", point.income),
                    series: .value("Series", props.incomeLabel)
                )
                .foregroundStyle(by: .value("Series", props.incomeLabel))
                .lineStyle(StrokeStyle(lineWidth: 2, lineCap: .round, lineJoin: .round))
                .symbol(Circle())
                .symbolSize(index == lastIndex ? 48 : 16)

                LineMark(
                    x: .value("Period", Double(index)),
                    y: .value("Amount", point.expenses),
                    series: .value("Series", props.expensesLabel)
                )
                .foregroundStyle(by: .value("Series", props.expensesLabel))
                .lineStyle(StrokeStyle(lineWidth: 2, lineCap: .round, lineJoin: .round, dash: [6, 4]))
                .symbol(Circle())
                .symbolSize(index == lastIndex ? 48 : 16)
            }

            if let index = selectedIndex, props.points.indices.contains(index) {
                let point = props.points[index]
                RuleMark(x: .value("Period", Double(index)))
                    .foregroundStyle(Color.secondary.opacity(0.6))
                    .lineStyle(StrokeStyle(lineWidth: 1))
                PointMark(x: .value("Period", Double(index)), y: .value("Amount", point.income))
                    .foregroundStyle(props.incomeColor)
                    .symbolSize(90)
                PointMark(x: .value("Period", Double(index)), y: .value("Amount", point.expenses))
                    .foregroundStyle(props.expensesColor)
                    .symbolSize(90)
            }

            if selectedIndex == nil, let last = props.points.last {
                PointMark(x: .value("Period", Double(lastIndex)), y: .value("Amount", last.income))
                    .opacity(0)
                    .annotation(position: .top, alignment: .trailing, spacing: 6) {
                        endLabel(currency(last.income), style: .primary)
                    }
                PointMark(x: .value("Period", Double(lastIndex)), y: .value("Amount", last.expenses))
                    .opacity(0)
                    .annotation(position: .top, alignment: .trailing, spacing: 6) {
                        endLabel(currency(last.expenses), style: .secondary)
                    }
            }
        }
        .chartXScale(domain: -0.5...(Double(max(lastIndex, 0)) + 0.5))
        .chartXAxis {
            AxisMarks(preset: .aligned, values: props.points.indices.map(Double.init)) { value in
                AxisGridLine()
                AxisValueLabel(anchor: .top) {
                    if let position = value.as(Double.self), props.points.indices.contains(Int(position)) {
                        Text(props.points[Int(position)].label)
                    }
                }
            }
        }
        .chartYScale(domain: 0...yUpperBound)
        .chartForegroundStyleScale([
            props.incomeLabel: props.incomeColor,
            props.expensesLabel: props.expensesColor,
            props.deficitLabel: props.deficitColor.opacity(0.25),
        ])
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
        .chartOverlay { proxy in
            GeometryReader { geometry in
                let plot = geometry[proxy.plotAreaFrame]
                ZStack(alignment: .topLeading) {
                    Rectangle()
                        .fill(.clear)
                        .contentShape(Rectangle())
                        .gesture(
                            LongPressGesture(minimumDuration: 0.2)
                                .sequenced(before: DragGesture(minimumDistance: 0))
                                .onChanged { value in
                                    if case .second(true, let drag?) = value {
                                        select(atPlotX: drag.location.x - plot.origin.x, proxy: proxy)
                                    }
                                }
                                .onEnded { _ in selectedIndex = nil }
                        )
                        .accessibilityHidden(true)

                    if let index = selectedIndex, props.points.indices.contains(index),
                       let x = proxy.position(forX: Double(index)) {
                        let halfWidth = tooltipWidth / 2
                        let centre = min(max(plot.origin.x + x, halfWidth), geometry.size.width - halfWidth)
                        tooltip(for: props.points[index])
                            .position(x: centre, y: plot.origin.y + 44)
                            .allowsHitTesting(false)
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
