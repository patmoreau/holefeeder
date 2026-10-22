import type { StyleProp, ViewStyle } from 'react-native';

export type IncomeExpenseChartPoint = {
  label: string;
  income: number;
  expenses: number;
};

export type IncomeExpenseChartProps = {
  points: IncomeExpenseChartPoint[];
  incomeLabel: string;
  expensesLabel: string;
  incomeColor: string;
  expensesColor: string;
  currencyCode: string;
  accessibilitySummary: string;
  identifier?: string;
  style?: StyleProp<ViewStyle>;
};
