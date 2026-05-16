import { Platform, TextStyle } from 'react-native';
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
  };
  typography: {
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

export const Fonts = Platform.select({
  ios: {
    /** iOS `UIFontDescriptorSystemDesignDefault` */
    sans: 'system-ui',
    /** iOS `UIFontDescriptorSystemDesignSerif` */
    serif: 'ui-serif',
    /** iOS `UIFontDescriptorSystemDesignRounded` */
    rounded: 'ui-rounded',
    /** iOS `UIFontDescriptorSystemDesignMonospaced` */
    mono: 'ui-monospace',
  },
  default: {
    sans: 'normal',
    serif: 'serif',
    rounded: 'normal',
    mono: 'monospace',
  },
  web: {
    sans: "system-ui, -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Helvetica, Arial, sans-serif",
    serif: "Georgia, 'Times New Roman', serif",
    rounded: "'SF Pro Rounded', 'Hiragino Maru Gothic ProN', Meiryo, 'MS PGothic', sans-serif",
    mono: "SFMono-Regular, Menlo, Monaco, Consolas, 'Liberation Mono', 'Courier New', monospace",
  },
});
