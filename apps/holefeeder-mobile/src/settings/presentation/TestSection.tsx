import { router } from 'expo-router';
import { useTranslation } from 'react-i18next';
import { tk } from '@/i18n/translations';
import { AppField } from '@/shared/presentation/AppField';
import { AppSection } from '@/shared/presentation/AppSection';
import { AppButton } from '@/shared/presentation/components/AppButton';
import { AppIcons } from '@/shared/presentation/icons';

export const TestSection = () => {
  const { t } = useTranslation();

  return (
    <AppSection title={t(tk.testSection.title)}>
      <AppField label={t(tk.testSection.notFoundPage)} icon={AppIcons.warning}>
        <AppButton
          label={t(tk.testSection.goTo)}
          variant={'link'}
          onPress={() => {
            // @ts-ignore
            router.push({ pathname: '/+not-found' });
          }}
        />
      </AppField>
      <AppField label={t(tk.testSection.testComponents)} icon={AppIcons.warning}>
        <AppButton
          label={t(tk.testSection.component)}
          variant={'link'}
          onPress={() => {
            // @ts-ignore
            router.push({ pathname: '/test' });
          }}
        />
      </AppField>
    </AppSection>
  );
};
