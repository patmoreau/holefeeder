import { darkTheme, lightTheme, ThemeMode } from '@/types/theme';

export type ThemeState = {
  theme: typeof lightTheme | typeof darkTheme;
  isDark: boolean;
  themeMode: ThemeMode;
  setThemeMode: (theme: ThemeMode) => void;
  availableThemeModes: { code: ThemeMode; langId: string }[];
};
