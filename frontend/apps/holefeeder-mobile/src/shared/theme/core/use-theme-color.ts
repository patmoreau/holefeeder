import { useTheme } from '@/shared/theme/core/use-theme';
import { darkTheme } from '@/types/theme/dark';
import { lightTheme } from '@/types/theme/light';

export const useThemeColor = (
  props: { light?: string; dark?: string },
  colorName: keyof typeof lightTheme.colors & keyof typeof darkTheme.colors
): string => {
  const { theme, isDark } = useTheme();
  const colorFromProps = isDark ? props['dark'] : props['light'];

  if (colorFromProps) {
    return colorFromProps;
  } else {
    return theme.colors[colorName];
  }
};
