import React from 'react';
import { useTranslation } from 'react-i18next';
import { tk } from '@/i18n/translations';
import { AppButton } from '@/shared/presentation/components/native/AppButton';
import { AppColumn } from '@/shared/presentation/components/native/AppColumn';
import { AppNative } from '@/shared/presentation/components/native/AppNative';
import { AppText } from '@/shared/presentation/components/native/AppText';
import { useTheme } from '@/shared/theme/core/use-theme';

// The gate cannot tell whether the caller is registered, so it must not guess: sending
// them to onboarding would re-run signup for an existing user, and sending them to the
// app would show an empty one. Ask them to retry instead.
export const RegistrationError = ({ onRetry }: { onRetry: () => void }) => {
  const { t } = useTranslation();
  const { theme } = useTheme();

  return (
    <AppNative style={{ flex: 1 }} testID="registration-error">
      <AppColumn spacing={8} alignment={'center'}>
        <AppText variant={'title'} textStyle={{ color: theme.colors.secondaryText }}>
          {t(tk.errors.cannotReachServer.title)}
        </AppText>
        <AppText variant={'subtitle'} textStyle={{ color: theme.colors.secondaryText }}>
          {t(tk.errors.cannotReachServer.message)}
        </AppText>
        <AppButton label={t(tk.common.retry)} variant="primary" onPress={onRetry} testID="registration-error-retry-button" />
      </AppColumn>
    </AppNative>
  );
};
