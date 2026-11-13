import { Palette } from '@holefeeder/shared/theme';
import { TextStyle } from 'react-native';
import { createBaseStyles } from '@/types/theme/base-styles';

export const ThemeMode = {
  light: 'light',
  dark: 'dark',
  system: 'system',
} as const;

export type ThemeMode = (typeof ThemeMode)[keyof typeof ThemeMode];

export type Typography = 'default' | 'title';

export interface Theme {
  colors: Palette;
  typography: {
    display: TextStyle;
    largeTitle: TextStyle;
    title: TextStyle;
    subtitle: TextStyle;
    body: TextStyle;
    secondary: TextStyle;
    footnote: TextStyle;
    errorField: TextStyle;
  };
  styles: ReturnType<typeof createBaseStyles> & {};
}
