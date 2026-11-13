export type Palette = {
  primary: string;
  secondary: string;
  background: string;
  secondaryBackground: string;
  text: string;
  primaryText: string;
  secondaryText: string;
  destructive: string;
  error: string;
  tint: string;
  icon: string;
  tabIconDefault: string;
  tabIconSelected: string;
  separator: string;
  opaqueSeparator: string;
  link: string;
  dashboard: string;
  accounts: string;
  settings: string;
  positive: string;
  positiveBackground: string;
  negative: string;
  negativeBackground: string;
  amountNeutral: string;
  chartIncome: string;
  chartExpenses: string;
};

const light: Palette = {
  primary: '#7B42F6',
  secondary: '#FF8C42',
  background: '#FFFFFF',
  secondaryBackground: '#F2F2F7',
  text: '#1C1C1E',
  primaryText: '#FFFFFF',
  secondaryText: '#6C6C70',
  destructive: '#E53B3B',
  error: '#FF3B30',
  tint: '#7B42F6',
  icon: '#6C6C70',
  tabIconDefault: '#687076',
  tabIconSelected: '#7B42F6',
  separator: '#C6C6C8',
  opaqueSeparator: '#C6C6C8',
  link: '#7B42F6',
  dashboard: '#F0E6F7',
  accounts: '#FFF5EB',
  settings: '#F2F2F7',
  positive: '#2E7D32',
  positiveBackground: '#E6F7ED',
  negative: '#D32F2F',
  negativeBackground: '#FFF2F2',
  amountNeutral: '#1C1C1E',
  chartIncome: '#2E7D32',
  chartExpenses: '#FF8C42',
};

const dark: Palette = {
  primary: '#9D6DE6',
  secondary: '#FF9E66',
  background: '#000000',
  secondaryBackground: '#1C1C1E',
  text: '#F2F2F7',
  primaryText: '#F2F2F7',
  secondaryText: '#8E8E93',
  destructive: '#FF453A',
  error: '#FF453A',
  tint: '#9D6DE6',
  icon: '#AEAEB2',
  tabIconDefault: '#AEAEB2',
  tabIconSelected: '#9D6DE6',
  separator: '#38383A',
  opaqueSeparator: '#38383A',
  link: '#9D6DE6',
  dashboard: '#2E1A47',
  accounts: '#472F1A',
  settings: '#1C1C1E',
  positive: '#7CB342',
  positiveBackground: '#1E3A2E',
  negative: '#FF6F42',
  negativeBackground: '#3A1E1E',
  amountNeutral: '#F2F2F7',
  chartIncome: '#26A69A',
  chartExpenses: '#D96F26',
};

export const Palettes = {
  light: light,
  dark: dark,
} as const;

export const BrandColors = {
  orange: '#FF8D28',
} as const;
