import { createBaseStyles } from '@/types/theme/base-styles';
import { lightTheme } from '@/types/theme/light';
import { Theme } from '@/types/theme/theme';

const colors = {
  primary: '#9D6DE6',
  secondary: '#FF9E66',
  background: '#000000',
  secondaryBackground: '#1C1C1E',
  text: '#F2F2F7',
  primaryText: '#000000',
  secondaryText: '#2b2b2d',
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
};

export const darkTheme: Theme = {
  ...lightTheme,
  colors,
  styles: {
    // New base styles using design tokens with dark theme colors
    ...createBaseStyles({ colors, typography: lightTheme.typography } as any),
    // Legacy support
    ...lightTheme.styles,
  },
};
