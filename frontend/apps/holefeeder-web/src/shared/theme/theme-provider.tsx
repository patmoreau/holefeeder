import { CssBaseline, ThemeProvider, createTheme } from '@mui/material';
import type { ReactNode } from 'react';

const theme = createTheme({
  cssVariables: { colorSchemeSelector: 'class' },
  colorSchemes: {
    light: {
      palette: {
        primary: { main: '#7B42F6', contrastText: '#FFFFFF' },
        success: { main: '#2E7D32' },
        error: { main: '#D32F2F' },
        background: { default: '#FFFFFF', paper: '#F2F2F7' },
        text: { primary: '#1C1C1E', secondary: '#6C6C70' },
        divider: '#C6C6C8',
      },
    },
    dark: {
      palette: {
        primary: { main: '#9D6DE6', contrastText: '#F2F2F7' },
        success: { main: '#7CB342' },
        error: { main: '#FF6F42' },
        background: { default: '#000000', paper: '#1C1C1E' },
        text: { primary: '#F2F2F7', secondary: '#8E8E93' },
        divider: '#38383A',
      },
    },
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
