import { QueryClientProvider } from '@tanstack/react-query';
import { StrictMode } from 'react';
import { createRoot } from 'react-dom/client';
import { Router } from './router';
import { queryClient } from './shared/api/query-client';
import { AuthenticationProvider } from './shared/auth/authentication-provider';
import { AppThemeProvider } from './shared/theme/theme-provider';

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <AuthenticationProvider>
      <QueryClientProvider client={queryClient}>
        <AppThemeProvider>
          <Router />
        </AppThemeProvider>
      </QueryClientProvider>
    </AuthenticationProvider>
  </StrictMode>
);
