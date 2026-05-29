import { router } from 'expo-router';
import { useTranslation } from 'react-i18next';
import { tk } from '@/i18n/translations';
import { AppIconMap } from '@/shared/presentation/components/app/app-icon-map';
import { AppButton } from '@/shared/presentation/components/app/AppButton';
import { AppField } from '@/shared/presentation/components/app/AppField';
import { AppFieldSection } from '@/shared/presentation/components/app/AppFieldSection';

export function SyncSection() {
  const { t } = useTranslation();

  return (
    <AppFieldSection title={t(tk.settings.syncSection.title)}>
      <AppField label={t(tk.settings.syncSection.title)} icon={AppIconMap.sync}>
        <AppButton
          label={t(tk.settings.syncSection.navigation)}
          icon={AppIconMap.expand}
          iconPosition={'right'}
          variant={'link'}
          onPress={() => router.push('/(app)/SyncSettings')}
        />
      </AppField>
    </AppFieldSection>
  );
}
