import { useTranslation } from 'react-i18next';
import { tk } from '@/i18n/translations';
import { useAuth } from '@/shared/auth/core/use-auth';
import { AppFieldButton } from '@/shared/presentation/components/native/AppFieldButton';
import { AppFieldSection } from '@/shared/presentation/components/native/AppFieldSection';

export const LogoutSection = () => {
  const { logout } = useAuth();
  const { t } = useTranslation();

  return (
    <AppFieldSection>
      <AppFieldButton label={t(tk.auth.logoutButton)} onPress={logout} destructive testID="auth-logout-button" />
    </AppFieldSection>
  );
};
