import { Platform } from 'react-native';
import { IncomeExpenseChart, IncomeExpenseChartProps } from '@/modules/income-expense-chart';

export const IncomeExpenseChartView = (props: IncomeExpenseChartProps) => {
  if (Platform.OS !== 'ios') return null;

  return <IncomeExpenseChart {...props} />;
};
