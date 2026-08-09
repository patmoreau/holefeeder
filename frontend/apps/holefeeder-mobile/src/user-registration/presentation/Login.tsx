import React from 'react';
import { useTranslation } from 'react-i18next';
import { tk } from '@/i18n/translations';
import { useAuth } from '@/shared/auth/core/use-auth';
import { AuthButton } from '@/shared/presentation/AuthButton';
import { AppColumn } from '@/shared/presentation/components/native/AppColumn';
import { AppIcon } from '@/shared/presentation/components/native/AppIcon';
import { AppLoadingIndicator } from '@/shared/presentation/components/native/AppLoadingIndicator';
import { AppNative } from '@/shared/presentation/components/native/AppNative';
import { AppText } from '@/shared/presentation/components/native/AppText';
import { AppIconMap } from '@/shared/presentation/core/app-icon-map';
import { useTheme } from '@/shared/theme/core/use-theme';

const LoginScreen = () => {
  const { isLoading } = useAuth();
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
    <AppNative style={{ flex: 1 }} testID="login-screen">
      <AppColumn spacing={8} alignment={'center'}>
        <AppText variant={'title'} textStyle={{ color: theme.colors.secondaryText }}>
          {t(tk.auth.loginTitle)}
        </AppText>
        <AppText variant={'subtitle'} textStyle={{ color: theme.colors.secondaryText }}>
          {t(tk.auth.loginSubtitle)}
        </AppText>
        <AuthButton />
        <AppIcon name={AppIconMap.warning} style={{ paddingTop: 16 }} color={theme.colors.primary} />
      </AppColumn>
    </AppNative>
  );
};

export default LoginScreen;
