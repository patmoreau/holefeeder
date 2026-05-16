import { router } from 'expo-router';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { tk } from '@/i18n/translations';
import { AppField } from '@/shared/presentation/AppField';
import { AppSection } from '@/shared/presentation/AppSection';
import { AppButton } from '@/shared/presentation/components/AppButton';
import { AppIcons } from '@/shared/presentation/icons';

export function SyncSection() {
  const { t } = useTranslation();

  return (
    <AppSection title={t(tk.settings.syncSection.title)}>
      <AppField label={t(tk.settings.syncSection.title)} icon={AppIcons.sync}>
        <AppButton
          label={t(tk.settings.syncSection.navigation)}
          icon={AppIcons.expand}
          iconPosition={'right'}
          variant={'link'}
          onPress={() => {
            router.push('/(app)/SyncSettings');
          }}
        />
      </AppField>
    </AppSection>
  );
}
