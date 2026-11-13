import { Palettes } from '@holefeeder/shared/theme';
import { createBaseStyles } from '@/types/theme/base-styles';
import { lightTheme } from '@/types/theme/light';
import { Theme } from '@/types/theme/theme';

const colors = Palettes.dark;

export const darkTheme: Theme = {
  ...lightTheme,
  colors,
  styles: {
    // Light styles as fallback, then dark overrides win
    ...lightTheme.styles,
    ...createBaseStyles({ colors, typography: lightTheme.typography } as unknown as Theme),
  },
};
