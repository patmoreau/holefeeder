import { requireNativeView } from 'expo';
import { IncomeExpenseChartProps } from './IncomeExpenseChart.types';

const NativeView = requireNativeView<IncomeExpenseChartProps>('IncomeExpenseChart');

export default function IncomeExpenseChart(props: IncomeExpenseChartProps) {
  return <NativeView {...props} />;
}
