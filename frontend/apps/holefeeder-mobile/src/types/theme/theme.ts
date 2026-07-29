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
  colors: {
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

    // separators
    separator: string;
    opaqueSeparator: string;

    // others
    link: string;

    // tabs
    dashboard: string;
    accounts: string;
    settings: string;

    // amounts
    positive: string;
    positiveBackground: string;
    negative: string;
    negativeBackground: string;
    amountNeutral: string;
  };
  typography: {
    display: TextStyle;
    largeTitle: TextStyle;
    title: TextStyle;
    subtitle: TextStyle;
    body: TextStyle;
    secondary: TextStyle;
    footnote: TextStyle;
    chip: TextStyle;
    errorField: TextStyle;
  };
  styles: ReturnType<typeof createBaseStyles> & {};
}
