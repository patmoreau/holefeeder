import React from 'react';
import { aLightThemeState } from '@/shared/theme/__tests__/theme-state-for-test';
import { ThemeState } from '@/shared/theme/core/theme-state';
import { ThemeContext } from '@/shared/theme/presentation/ThemeProvider';

export const ThemeProviderForTest = ({ children, overrides }: { children: React.ReactNode; overrides?: Partial<ThemeState> }) => {
  return <ThemeContext.Provider value={aLightThemeState(overrides)}>{children}</ThemeContext.Provider>;
};
