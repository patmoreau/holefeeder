import { ThemeState } from '@/shared/theme/core/theme-state';
import { darkTheme, lightTheme, ThemeMode } from '@/types/theme';

const defaultTheme = (): ThemeState => ({
  theme: lightTheme,
  isDark: false,
  themeMode: ThemeMode.light,
  setThemeMode: (themeMode: ThemeMode) => {},
  availableThemeModes: [
    { code: ThemeMode.light, langId: 'light' },
    { code: ThemeMode.dark, langId: 'dark' },
    { code: ThemeMode.system, langId: 'system' },
  ],
});

export const aLightThemeState = (overrides?: Partial<ThemeState>): ThemeState => ({
  ...defaultTheme(),
  ...overrides,
});

export const aDarkThemeState = (overrides?: Partial<ThemeState>): ThemeState => ({
  ...defaultTheme(),
  theme: darkTheme,
  isDark: true,
  themeMode: ThemeMode.dark,
  ...overrides,
});
