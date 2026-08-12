import React from 'react';
import { useTranslation } from 'react-i18next';
import { tk } from '@/i18n/translations';
import { useAuth } from '@/shared/auth/core/use-auth';
import { AppButton } from '@/shared/presentation/components/native/AppButton';
import { AppColumn } from '@/shared/presentation/components/native/AppColumn';
import { AppLoadingIndicator } from '@/shared/presentation/components/native/AppLoadingIndicator';
import { AppNative } from '@/shared/presentation/components/native/AppNative';
import { AppText } from '@/shared/presentation/components/native/AppText';
import { useTheme } from '@/shared/theme/core/use-theme';

const WelcomeScreen = () => {
  const { isLoading, login } = useAuth();
  const { theme } = useTheme();

  const { t } = useTranslation();

  if (isLoading) {
    return (
      <AppNative style={{ flex: 1 }}>
        <AppLoadingIndicator size="large" />
      </AppNative>
    );
  }

  return (
    <AppNative style={{ flex: 1 }} testID="welcome-screen">
      <AppColumn spacing={8} alignment={'center'}>
        <AppText variant={'title'} textStyle={{ color: theme.colors.secondaryText }}>
          {t(tk.auth.welcomeTitle)}
        </AppText>
        <AppText variant={'subtitle'} textStyle={{ color: theme.colors.secondaryText }}>
          {t(tk.auth.welcomeSubtitle)}
        </AppText>
        {/* Both open the same Auth0 flow; only the page it lands on differs. */}
        <AppButton
          label={t(tk.auth.createAccountButton)}
          variant="primary"
          onPress={() => login({ signUp: true })}
          testID="welcome-signup-button"
        />
        <AppButton label={t(tk.auth.signInButton)} variant="secondary" onPress={() => login()} testID="welcome-signin-button" />
      </AppColumn>
    </AppNative>
  );
};

export default WelcomeScreen;
