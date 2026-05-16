import { useContext } from 'react';
import { ThemeState } from '@/shared/theme/core/theme-state';
import { ThemeContext } from '@/shared/theme/presentation/ThemeProvider';

export function useTheme(): ThemeState {
  const state = useContext(ThemeContext);
  if (!state) {
    throw new Error('useTheme must be used within a ThemeProvider');
  }
  return state;
}
