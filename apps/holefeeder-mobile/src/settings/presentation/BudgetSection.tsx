import { router } from 'expo-router';
import { useTranslation } from 'react-i18next';
import { tk } from '@/i18n/translations';
import { AppButton } from '@/shared/presentation/components/native/AppButton';
import { AppField } from '@/shared/presentation/components/native/AppField';
import { AppFieldSection } from '@/shared/presentation/components/native/AppFieldSection';
import { AppIconMap } from '@/shared/presentation/core/app-icon-map';

export function BudgetSection() {
  const { t } = useTranslation();

  return (
    <AppFieldSection title={t(tk.budgetSection.title)}>
      <AppField label={t(tk.budgetSection.budget)} icon={AppIconMap.settings}>
        <AppButton
          label={t(tk.budgetSection.settings)}
          icon={AppIconMap.expand}
          iconPosition={'right'}
          variant={'link'}
          onPress={() => {
            router.push('/(app)/BudgetSettings');
          }}
        />
      </AppField>
    </AppFieldSection>
  );
}
