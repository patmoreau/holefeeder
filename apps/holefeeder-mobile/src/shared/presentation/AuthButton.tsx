import * as React from 'react';
import { useTranslation } from 'react-i18next';
import { tk } from '@/i18n/translations';
import { useAuth } from '@/shared/auth/core/use-auth';
import { AppButton } from '@/shared/presentation/components/AppButton';

export const AuthButton = () => {
  const { user, login, logout } = useAuth();
  const { t } = useTranslation();

  if (user) {
    return <AppButton label={t(tk.auth.logoutButton)} variant="destructive" onPress={logout} />;
  }

  return <AppButton label={t(tk.auth.loginButton)} variant="primary" onPress={login} />;
};
