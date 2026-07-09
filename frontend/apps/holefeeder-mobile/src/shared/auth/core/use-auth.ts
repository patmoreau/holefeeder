import { useContext } from 'react';
import { AuthenticationContext } from '@/shared/auth/presentation/AuthenticationProvider';

export function useAuth() {
  const context = useContext(AuthenticationContext);
  if (!context) {
    throw new Error('useAuth must be used within an AuthenticationProvider');
  }
  return context;
}
