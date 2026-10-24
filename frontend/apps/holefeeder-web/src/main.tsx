import { QueryClientProvider } from '@tanstack/react-query';
import { StrictMode } from 'react';
import { createRoot } from 'react-dom/client';
import { Router } from './router';
import { queryClient } from './shared/api/query-client';
import { AuthenticationProvider } from './shared/auth/authentication-provider';
import { E2eConfig } from './shared/auth/e2e-config';
import { E2eAuthenticationProvider } from './shared/auth/E2eAuthenticationProvider';
import { AppThemeProvider } from './shared/theme/theme-provider';

const AuthProvider = E2eConfig.isEnabled() ? E2eAuthenticationProvider : AuthenticationProvider;

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <AuthProvider>
      <QueryClientProvider client={queryClient}>
        <AppThemeProvider>
          <Router />
        </AppThemeProvider>
      </QueryClientProvider>
    </AuthProvider>
  </StrictMode>
);
