import { router } from 'expo-router';
import { useTranslation } from 'react-i18next';
import { tk } from '@/i18n/translations';
import { AppButton } from '@/shared/presentation/components/native/AppButton';
import { AppField } from '@/shared/presentation/components/native/AppField';
import { AppFieldSection } from '@/shared/presentation/components/native/AppFieldSection';
import { AppIconMap } from '@/shared/presentation/core/app-icon-map';

export function TagsSection() {
  const { t } = useTranslation();

  return (
    <AppFieldSection title={t(tk.tagsSection.title)}>
      <AppField label={t(tk.tagsSection.title)} icon={AppIconMap.tag}>
        <AppButton
          label={t(tk.tagsSection.manage)}
          icon={AppIconMap.expand}
          iconPosition={'right'}
          variant={'link'}
          onPress={() => {
            router.push('/(app)/ManageTags');
          }}
        />
      </AppField>
    </AppFieldSection>
  );
}
