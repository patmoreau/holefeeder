import { QueryClientProvider } from '@tanstack/react-query';
import { StrictMode } from 'react';
import { createRoot } from 'react-dom/client';
import { AuthenticationProvider } from './shared/auth/authentication-provider';
import { queryClient } from './shared/api/query-client';
import { AppThemeProvider } from './shared/theme/theme-provider';
import { Router } from './router';

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <AuthenticationProvider>
      <QueryClientProvider client={queryClient}>
        <AppThemeProvider>
          <Router />
        </AppThemeProvider>
      </QueryClientProvider>
    </AuthenticationProvider>
  </StrictMode>,
);
