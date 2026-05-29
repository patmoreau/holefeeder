import React from 'react';
import { useTranslation } from 'react-i18next';
import { tk } from '@/i18n/translations';
import { useAuth } from '@/shared/auth/core/use-auth';
import { AuthButton } from '@/shared/presentation/AuthButton';
import { AppIconMap } from '@/shared/presentation/components/app/app-icon-map';
import { AppColumn } from '@/shared/presentation/components/app/AppColumn';
import { AppHost } from '@/shared/presentation/components/app/AppHost';
import { AppIcon } from '@/shared/presentation/components/app/AppIcon';
import { AppLoadingIndicator } from '@/shared/presentation/components/app/AppLoadingIndicator';
import { AppText } from '@/shared/presentation/components/app/AppText';
import { useTheme } from '@/shared/theme/core/use-theme';

const LoginScreen = () => {
  const { isLoading } = useAuth();
  const { theme } = useTheme();

  const { t } = useTranslation();

  if (isLoading) {
    return (
      <AppHost style={{ flex: 1 }}>
        <AppLoadingIndicator size="large" />
      </AppHost>
    );
  }

  return (
    <AppHost style={{ flex: 1 }}>
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
    </AppHost>
  );
};

export default LoginScreen;
