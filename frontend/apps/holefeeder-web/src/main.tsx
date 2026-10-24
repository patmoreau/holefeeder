import { StrictMode } from 'react';
import { createRoot } from 'react-dom/client';
import { Router } from './router';
import { AuthenticationProvider } from './shared/auth/authentication-provider';
import { E2eConfig } from './shared/auth/e2e-config';
import { E2eAuthenticationProvider } from './shared/auth/E2eAuthenticationProvider';
import { AppThemeProvider } from './shared/theme/theme-provider';

const AuthProvider = E2eConfig.isEnabled() ? E2eAuthenticationProvider : AuthenticationProvider;

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <AuthProvider>
      <AppThemeProvider>
        <Router />
      </AppThemeProvider>
    </AuthProvider>
  </StrictMode>
);
