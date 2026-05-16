import { useMemo } from 'react';
import { StyleSheet } from 'react-native';
import { useTheme } from '@/shared/theme/core/use-theme';
import { GlobalStyles } from '@/types/theme/global-styles';
import { Theme } from '@/types/theme/theme';
import NamedStyles = StyleSheet.NamedStyles;

type StyleFunction<T> = (theme: Theme, globalStyles: typeof GlobalStyles) => T;

export const useStyles = <T extends NamedStyles<T>>(stylesFn: StyleFunction<T>): T => {
  const themeState = useTheme();
  const { theme } = themeState;

  return useMemo(() => stylesFn(theme, GlobalStyles), [stylesFn, theme]);
};

const Alignment: Record<string, string> = {
  center: 'center',
  'flex-start': 'leading',
  'flex-end': 'trailing',
};

export const iosAlignmentStyle = (align: 'center' | 'flex-start' | 'flex-end') => ({ alignment: Alignment[align] });
