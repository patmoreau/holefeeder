import type { StyleProp, ViewStyle } from 'react-native';

export type IncomeExpenseChartPoint = {
  label: string;
  income: number;
  expenses: number;
  tooltipTitle: string;
  netText: string;
  netTone: 'positive' | 'negative' | 'neutral';
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
  positiveColor: string;
  negativeColor: string;
  currencyCode: string;
  accessibilitySummary: string;
  identifier?: string;
  style?: StyleProp<ViewStyle>;
};
