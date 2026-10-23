import { router } from 'expo-router';
import React, { useEffect } from 'react';
import { useTranslation } from 'react-i18next';
import { tk } from '@/i18n/translations';
import { BrandBackground } from '@/shared/presentation/components/BrandBackground';
import { BrandTile } from '@/shared/presentation/components/BrandTile';
import { AppButton } from '@/shared/presentation/components/native/AppButton';
import { AppColumn } from '@/shared/presentation/components/native/AppColumn';
import { AppLoadingIndicator } from '@/shared/presentation/components/native/AppLoadingIndicator';
import { AppModifiers } from '@/shared/presentation/components/native/AppModifiers';
import { AppNative } from '@/shared/presentation/components/native/AppNative';
import { AppText } from '@/shared/presentation/components/native/AppText';
import { useTheme } from '@/shared/theme/core/use-theme';
import { useOnboardingRegistration } from '@/user-registration/presentation/core/use-onboarding-registration';

const RegisteringScreen = () => {
  const { progress, ready, retry } = useOnboardingRegistration();
  const { t } = useTranslation();
  const { theme } = useTheme();

  // replace, not push: registering is done and must not be reachable by going back.
  useEffect(() => {
    if (ready) {
      router.replace('/BudgetPeriod');
    }
  }, [ready]);

  if (progress.isFailure) {
    return (
      <BrandBackground testID="onboarding-registering-failed">
        <AppNative style={{ flex: 1 }}>
          <AppColumn spacing={8} alignment={'center'}>
            <BrandTile testID="onboarding-registering-brand-tile" />
            <AppText variant={'title'} textStyle={{ color: theme.colors.text }}>
              {t(tk.onboarding.registeringFailedTitle)}
            </AppText>
            <AppText variant={'subtitle'} textStyle={{ color: theme.colors.secondaryText }} modifiers={[AppModifiers.centerText]}>
              {t(tk.onboarding.registeringFailedMessage)}
            </AppText>
            <AppButton label={t(tk.common.retry)} variant="primary" onPress={retry} testID="onboarding-registering-retry-button" />
          </AppColumn>
        </AppNative>
      </BrandBackground>
    );
  }

  return (
    <BrandBackground testID="onboarding-registering-screen">
      <AppNative style={{ flex: 1 }}>
        <AppColumn spacing={8} alignment={'center'}>
          <BrandTile testID="onboarding-registering-brand-tile" />
          <AppText variant={'title'} textStyle={{ color: theme.colors.text }}>
            {t(tk.onboarding.registeringTitle)}
          </AppText>
          <AppText variant={'subtitle'} textStyle={{ color: theme.colors.secondaryText }} modifiers={[AppModifiers.centerText]}>
            {t(tk.onboarding.registeringSubtitle)}
          </AppText>
          <AppLoadingIndicator size="large" />
        </AppColumn>
      </AppNative>
    </BrandBackground>
  );
};

export default RegisteringScreen;
