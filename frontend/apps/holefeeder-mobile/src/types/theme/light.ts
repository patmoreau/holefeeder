import { Palettes } from '@holefeeder/shared/theme';
import { createBaseStyles } from '@/types/theme/base-styles';
import { fontSize, fontWeight, lineHeight } from '@/types/theme/design-tokens';
import { Theme } from '@/types/theme/theme';

// Typography using design tokens
const typography = {
  largeTitle: {
    fontSize: fontSize!['3xl'],
    fontWeight: fontWeight.bold,
    lineHeight: fontSize!['3xl'] * lineHeight.tight,
  },
  title: {
    fontSize: fontSize!.xl,
    fontWeight: fontWeight.semiBold,
    lineHeight: fontSize!.xl * lineHeight.normal,
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
  errorField: {
    fontSize: fontSize!.sm,
  },
  display: {
    fontSize: fontSize!['4xl'] ?? fontSize!['3xl'] * 1.33,
    fontWeight: fontWeight.bold,
    lineHeight: (fontSize!['4xl'] ?? fontSize!['3xl'] * 1.33) * lineHeight.tight,
  },
} as const;

const colors = Palettes.light;

export const lightTheme: Theme = {
  colors,
  typography: typography,
  styles: {
    // New base styles using design tokens
    ...createBaseStyles({ colors, typography: typography } as unknown as Theme),
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
