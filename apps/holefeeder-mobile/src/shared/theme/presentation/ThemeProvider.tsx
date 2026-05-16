import * as SystemUI from 'expo-system-ui';
import React, { createContext, useCallback, useEffect, useState } from 'react';
import { Appearance } from 'react-native';
import { Logger } from '@/shared/core/logger/logger';
import { AppStorage } from '@/shared/persistence/app-storage';
import { ThemeState } from '@/shared/theme/core/theme-state';
import { darkTheme, lightTheme, ThemeMode } from '@/types/theme';

const logger = Logger.create('ThemeProvider');

const APP_SETTINGS_THEME_KEY = 'app_settings_theme';

const availableThemeModes = [
  { code: 'system' as ThemeMode, langId: 'system' },
  { code: 'light' as ThemeMode, langId: 'light' },
  { code: 'dark' as ThemeMode, langId: 'dark' },
];

export const ThemeContext = createContext<ThemeState | undefined>(undefined);

export const ThemeProvider = ({ children, storage }: { children: React.ReactNode; storage: AppStorage }) => {
  const [themeMode, setThemeMode] = useState<ThemeMode>(() => (storage.getString(APP_SETTINGS_THEME_KEY) || ThemeMode.system) as ThemeMode);
  const [systemColorScheme, setSystemColorScheme] = useState(() => Appearance.getColorScheme());

  useEffect(() => {
    try {
      if (Appearance.setColorScheme) {
        Appearance.setColorScheme(themeMode === ThemeMode.system ? 'unspecified' : themeMode);
      }
      storage.setString(APP_SETTINGS_THEME_KEY, themeMode);
    } catch (error) {
      logger.error('Error changing theme:', error);
    }
  }, [themeMode, storage]);

  useEffect(() => {
    const subscription = Appearance.addChangeListener(({ colorScheme }) => {
      setSystemColorScheme(colorScheme);
    });
    return () => subscription?.remove();
  }, []);

  const getCurrentTheme = useCallback(() => {
    if (themeMode === ThemeMode.system) {
      return systemColorScheme === ThemeMode.dark ? darkTheme : lightTheme;
    }
    return themeMode === ThemeMode.dark ? darkTheme : lightTheme;
  }, [themeMode, systemColorScheme]);

  useEffect(() => {
    SystemUI.setBackgroundColorAsync(getCurrentTheme().colors.background).then((_) => {});
  }, [themeMode, systemColorScheme, getCurrentTheme]);

  const value: ThemeState = {
    theme: getCurrentTheme(),
    isDark: getCurrentTheme() === darkTheme,
    themeMode,
    setThemeMode,
    availableThemeModes,
  };

  return <ThemeContext.Provider value={value}>{children}</ThemeContext.Provider>;
};
