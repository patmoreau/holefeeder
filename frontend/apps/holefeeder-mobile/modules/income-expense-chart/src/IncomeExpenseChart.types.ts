import type { StyleProp, ViewStyle } from 'react-native';

export type IncomeExpenseChartPoint = {
  label: string;
  income: number;
  expenses: number;
};

export type IncomeExpenseChartBand = {
  kind: 'surplus' | 'deficit';
  points: { x: number; low: number; high: number }[];
};

export type IncomeExpenseChartProps = {
  points: IncomeExpenseChartPoint[];
  incomeLabel: string;
  expensesLabel: string;
  incomeColor: string;
  expensesColor: string;
  bands: IncomeExpenseChartBand[];
  deficitLabel: string;
  deficitColor: string;
  currencyCode: string;
  accessibilitySummary: string;
  identifier?: string;
  style?: StyleProp<ViewStyle>;
};
