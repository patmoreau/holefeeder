import { router } from 'expo-router';
import { useTranslation } from 'react-i18next';
import { tk } from '@/i18n/translations';
import { AppFieldLink } from '@/shared/presentation/components/native/AppFieldLink';
import { AppFieldSection } from '@/shared/presentation/components/native/AppFieldSection';
import { AppIconMap } from '@/shared/presentation/core/app-icon-map';

export function HolefeederSection() {
  const { t } = useTranslation();

  return (
    <AppFieldSection title={t(tk.budgetSection.title)}>
      <AppFieldLink label={t(tk.budgetSection.budget)} icon={AppIconMap.settings} onPress={() => router.push('/(app)/BudgetSettings')} />
      <AppFieldLink label={t(tk.categoriesSection.title)} icon={AppIconMap.category} onPress={() => router.push('/(app)/ManageCategories')} />
      <AppFieldLink label={t(tk.cashflowsSection.title)} icon={AppIconMap.cashflow} onPress={() => router.push('/(app)/ManageCashflows')} />
      <AppFieldLink label={t(tk.tagsSection.title)} icon={AppIconMap.tag} onPress={() => router.push('/(app)/ManageTags')} />
    </AppFieldSection>
  );
}
