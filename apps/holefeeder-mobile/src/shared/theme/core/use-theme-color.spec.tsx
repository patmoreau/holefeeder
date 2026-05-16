import { renderHook, waitFor } from '@testing-library/react-native';
import React from 'react';
import { aDarkThemeState, aLightThemeState } from '@/shared/theme/__tests__/theme-state-for-test';
import { ThemeProviderForTest } from '@/shared/theme/__tests__/ThemeProviderForTest';
import { ThemeState } from '@/shared/theme/core/theme-state';
import { useThemeColor } from '@/shared/theme/core/use-theme-color';
import { darkTheme } from '@/types/theme/dark';
import { lightTheme } from '@/types/theme/light';

describe('useThemeColor', () => {
  const createHook = async (themeState: ThemeState, hookToRender: () => unknown) =>
    await waitFor(() =>
      renderHook(hookToRender, {
        wrapper: ({ children }: { children: React.ReactNode }) => (
          <ThemeProviderForTest overrides={themeState}>{children}</ThemeProviderForTest>
        ),
      })
    );

  describe('when using color from props', () => {
    it('returns color provided for light theme', async () => {
      const themeState = aLightThemeState();
      const props = { light: '#ff0000', dark: '#00ff00' };
      const colorName = 'text';

      const { result } = await createHook(themeState, () => useThemeColor(props, colorName));

      expect(result.current).toBe('#ff0000');
    });

    it('should return color provided for dark theme', async () => {
      const themeState = aDarkThemeState();
      const props = { light: '#ff0000', dark: '#00ff00' };
      const colorName = 'text';

      const { result } = await createHook(themeState, () => useThemeColor(props, colorName));

      expect(result.current).toBe('#00ff00');
    });

    it('should return color from light theme when props not provided for light theme', async () => {
      const themeState = aLightThemeState();
      const props = { dark: '#00ff00' };
      const colorName = 'text';

      const { result } = await createHook(themeState, () => useThemeColor(props, colorName));

      expect(result.current).toBe(lightTheme.colors.text);
    });

    it('should return color from dark theme when props not provided for dark theme', async () => {
      const themeState = aDarkThemeState();
      const props = { light: '#ff0000' };
      const colorName = 'text';

      const { result } = await createHook(themeState, () => useThemeColor(props, colorName));

      expect(result.current).toBe(darkTheme.colors.text);
    });
  });

  describe('when using color from theme', () => {
    it('should return color from light theme when no light props', async () => {
      const themeState = aLightThemeState();
      const props = {};
      const colorName = 'text';

      const { result } = await createHook(themeState, () => useThemeColor(props, colorName));

      expect(result.current).toBe(lightTheme.colors.text);
    });

    it('should return color from Colors constant when props not provided for dark theme', async () => {
      const themeState = aDarkThemeState();
      const props = {};
      const colorName = 'text';

      const { result } = await createHook(themeState, () => useThemeColor(props, colorName));

      expect(result.current).toBe(darkTheme.colors.text);
    });
  });
});
