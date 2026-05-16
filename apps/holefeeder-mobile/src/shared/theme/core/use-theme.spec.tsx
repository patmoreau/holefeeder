import { renderHook, RenderHookResult, waitFor } from '@testing-library/react-native';
import React from 'react';
import { aLightThemeState } from '@/shared/theme/__tests__/theme-state-for-test';
import { ThemeProviderForTest } from '@/shared/theme/__tests__/ThemeProviderForTest';
import { ThemeState } from '@/shared/theme/core/theme-state';
import { useTheme } from '@/shared/theme/core/use-theme';

describe('useTheme', () => {
  const themeState = aLightThemeState();
  let hook: RenderHookResult<ThemeState, unknown>;

  const createHook = async () =>
    await waitFor(() =>
      renderHook(() => useTheme(), {
        wrapper: ({ children }: { children: React.ReactNode }) => (
          <ThemeProviderForTest overrides={themeState}>{children}</ThemeProviderForTest>
        ),
      })
    );

  beforeEach(async () => {
    hook = await createHook();
  });

  it('returns theme', () => {
    expect(hook.result.current.theme).toBe(themeState.theme);
  });

  it('returns isDark', () => {
    expect(hook.result.current.isDark).toBe(themeState.isDark);
  });

  it('should return themeMode', () => {
    expect(hook.result.current.themeMode).toBe(themeState.themeMode);
  });

  it('should return setThemeMode', () => {
    expect(hook.result.current.setThemeMode).toBe(themeState.setThemeMode);
  });

  it('should return availableThemeModes', () => {
    expect(hook.result.current.availableThemeModes).toBe(themeState.availableThemeModes);
  });
});
