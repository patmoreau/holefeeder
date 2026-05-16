import { createBaseStyles } from '@/types/theme/base-styles';
import { fontSize, fontWeight, lineHeight, spacing } from '@/types/theme/design-tokens';
import { Theme } from '@/types/theme/theme';

// Typography using design tokens
const typography = {
  largeTitle: {
    fontSize: fontSize!['3xl'],
    fontWeight: fontWeight.bold,
  },
  title: {
    fontSize: fontSize!.xl,
    fontWeight: fontWeight.semiBold,
  },
  subtitle: {
    fontSize: fontSize!.md,
    fontWeight: fontWeight.normal,
  },
  body: {
    fontSize: fontSize!.lg,
    fontWeight: fontWeight.normal,
    lineHeight: fontSize!.lg * lineHeight.normal,
  },
  footnote: {
    fontSize: fontSize!.sm,
    fontWeight: fontWeight.normal,
  },
  secondary: {
    fontSize: fontSize!.md,
    fontWeight: fontWeight.normal,
  },
  chip: {
    fontSize: fontSize!.base,
    fontWeight: fontWeight.normal,
    lineHeight: fontSize!.base * lineHeight.tight,
  },
  errorField: {
    fontSize: fontSize!.sm,
    marginTop: spacing.xs,
    marginLeft: 36 + spacing.sm,
    marginBottom: spacing.sm,
  },
} as const;

// Color palette
const colors = {
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
  negative: '#B82E15',
  negativeBackground: '#FFF2F2',
} as const;

export const lightTheme: Theme = {
  colors,
  typography: typography,
  styles: {
    // New base styles using design tokens
    ...createBaseStyles({ colors, typography: typography } as any),
  },
};

/*
Font weight reference:
 100 = Thin
 200 = Extra Light
 300 = Light
 400 = Normal/Regular
 500 = Medium
 600 = Semi Bold
 700 = Bold
 800 = Extra Bold
 900 = Black
 */
