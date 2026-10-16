import { useMemo } from 'react';
import type { ImageStyle, TextStyle, ViewStyle } from 'react-native';
import { useTheme } from '@/shared/theme/core/use-theme';
import { GlobalStyles } from '@/types/theme/global-styles';
import { Theme } from '@/types/theme/theme';

type NamedStyles<T> = { [P in keyof T]: ViewStyle | TextStyle | ImageStyle };

type StyleFunction<T> = (theme: Theme, globalStyles: typeof GlobalStyles) => T;

export const useStyles = <T extends NamedStyles<T>>(stylesFn: StyleFunction<T>): T => {
  const themeState = useTheme();
  const { theme } = themeState;

  return useMemo(() => stylesFn(theme, GlobalStyles), [stylesFn, theme]);
};
