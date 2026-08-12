import React, { createContext, useContext } from 'react';
import { ApiConfig } from '@/shared/api/api-config';
import { RegistrationState, useRegistrationStatus } from '@/user-registration/presentation/core/use-registration-status';

export const RegistrationContext = createContext<RegistrationState | undefined>(undefined);

// The gate reads the registration status and the onboarding screen changes it, so
// the two need the same instance rather than a hook call each.
export const RegistrationProvider = ({ children, apiConfig }: { children: React.ReactNode; apiConfig: ApiConfig }) => {
  const value = useRegistrationStatus(apiConfig);

  return <RegistrationContext.Provider value={value}>{children}</RegistrationContext.Provider>;
};

export const useRegistration = (): RegistrationState => {
  const context = useContext(RegistrationContext);
  if (!context) {
    throw new Error('useRegistration must be used within a RegistrationProvider');
  }
  return context;
};
