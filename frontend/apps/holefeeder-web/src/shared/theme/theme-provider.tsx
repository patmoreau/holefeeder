import { type Palette, Palettes } from '@holefeeder/shared/theme';
import { CssBaseline, ThemeProvider, createTheme } from '@mui/material';
import type { ReactNode } from 'react';

const muiPalette = (palette: Palette) => ({
  primary: { main: palette.primary, contrastText: palette.primaryText },
  success: { main: palette.positive, light: palette.positiveBackground },
  error: { main: palette.negative, light: palette.negativeBackground },
  background: { default: palette.background, paper: palette.secondaryBackground },
  text: { primary: palette.text, secondary: palette.secondaryText },
  divider: palette.separator,
});

const theme = createTheme({
  cssVariables: { colorSchemeSelector: 'class' },
  colorSchemes: {
    light: { palette: muiPalette(Palettes.light) },
    dark: { palette: muiPalette(Palettes.dark) },
  },
  shape: { borderRadius: 8 },
  typography: {
    fontFamily: '"Inter Tight", "Helvetica", "Arial", sans-serif',
    button: { textTransform: 'none', fontWeight: 600 },
  },
  components: {
    MuiPaper: { styleOverrides: { root: { backgroundImage: 'none' } } },
  },
});

export const AppThemeProvider = ({ children }: { children: ReactNode }) => (
  <ThemeProvider theme={theme} defaultMode="system">
    <CssBaseline />
    {children}
  </ThemeProvider>
);
