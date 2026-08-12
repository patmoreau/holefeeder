import React from 'react';
import { useTranslation } from 'react-i18next';
import { tk } from '@/i18n/translations';
import { AppColumn } from '@/shared/presentation/components/native/AppColumn';
import { AppNative } from '@/shared/presentation/components/native/AppNative';
import { AppText } from '@/shared/presentation/components/native/AppText';
import { useTheme } from '@/shared/theme/core/use-theme';

// Placeholder so the gate has somewhere to send an unregistered caller. B10 replaces
// this with the screen that actually calls register, shows progress, and retries.
const RegisteringScreen = () => {
  const { t } = useTranslation();
  const { theme } = useTheme();

  return (
    <AppNative style={{ flex: 1 }} testID="onboarding-registering-screen">
      <AppColumn spacing={8} alignment={'center'}>
        <AppText variant={'title'} textStyle={{ color: theme.colors.secondaryText }}>
          {t(tk.auth.loginTitle)}
        </AppText>
      </AppColumn>
    </AppNative>
  );
};

export default RegisteringScreen;
