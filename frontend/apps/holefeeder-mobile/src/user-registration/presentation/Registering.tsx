import React from 'react';
import { useTranslation } from 'react-i18next';
import { tk } from '@/i18n/translations';
import { AppButton } from '@/shared/presentation/components/native/AppButton';
import { AppColumn } from '@/shared/presentation/components/native/AppColumn';
import { AppLoadingIndicator } from '@/shared/presentation/components/native/AppLoadingIndicator';
import { AppNative } from '@/shared/presentation/components/native/AppNative';
import { AppText } from '@/shared/presentation/components/native/AppText';
import { useTheme } from '@/shared/theme/core/use-theme';
import { useOnboardingRegistration } from '@/user-registration/presentation/core/use-onboarding-registration';

const RegisteringScreen = () => {
  const { progress, retry } = useOnboardingRegistration();
  const { t } = useTranslation();
  const { theme } = useTheme();

  if (progress.isFailure) {
    return (
      <AppNative style={{ flex: 1 }} testID="onboarding-registering-failed">
        <AppColumn spacing={8} alignment={'center'}>
          <AppText variant={'title'} textStyle={{ color: theme.colors.secondaryText }}>
            {t(tk.onboarding.registeringFailedTitle)}
          </AppText>
          <AppText variant={'subtitle'} textStyle={{ color: theme.colors.secondaryText }}>
            {t(tk.onboarding.registeringFailedMessage)}
          </AppText>
          <AppButton label={t(tk.common.retry)} variant="primary" onPress={retry} testID="onboarding-registering-retry-button" />
        </AppColumn>
      </AppNative>
    );
  }

  return (
    <AppNative style={{ flex: 1 }} testID="onboarding-registering-screen">
      <AppColumn spacing={8} alignment={'center'}>
        <AppText variant={'title'} textStyle={{ color: theme.colors.secondaryText }}>
          {t(tk.onboarding.registeringTitle)}
        </AppText>
        <AppText variant={'subtitle'} textStyle={{ color: theme.colors.secondaryText }}>
          {t(tk.onboarding.registeringSubtitle)}
        </AppText>
        <AppLoadingIndicator size="large" />
      </AppColumn>
    </AppNative>
  );
};

export default RegisteringScreen;
