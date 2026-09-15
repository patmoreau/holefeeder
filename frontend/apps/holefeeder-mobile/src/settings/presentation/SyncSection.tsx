import { router } from 'expo-router';
import { useTranslation } from 'react-i18next';
import { tk } from '@/i18n/translations';
import { AppFieldLink } from '@/shared/presentation/components/native/AppFieldLink';
import { AppFieldSection } from '@/shared/presentation/components/native/AppFieldSection';
import { AppIconMap } from '@/shared/presentation/core/app-icon-map';

export function SyncSection() {
  const { t } = useTranslation();

  return (
    <AppFieldSection title={t(tk.settings.syncSection.title)}>
      <AppFieldLink label={t(tk.settings.syncSection.title)} icon={AppIconMap.sync} onPress={() => router.push('/(app)/SyncSettings')} />
    </AppFieldSection>
  );
}
