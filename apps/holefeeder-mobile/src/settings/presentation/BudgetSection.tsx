import { router } from 'expo-router';
import { useTranslation } from 'react-i18next';
import { tk } from '@/i18n/translations';
import { AppIconMap } from '@/shared/presentation/components/app/app-icon-map';
import { AppButton } from '@/shared/presentation/components/app/AppButton';
import { AppField } from '@/shared/presentation/components/app/AppField';
import { AppFieldSection } from '@/shared/presentation/components/app/AppFieldSection';

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
