import { useTranslation } from 'react-i18next';
import { tk } from '@/i18n/translations';
import { AppFieldSection } from '@/shared/presentation/components/native/AppFieldSection';
import { AppListForEach } from '@/shared/presentation/components/native/AppListForEach';
import { AppListSectionTitle } from '@/shared/presentation/components/native/AppListSectionTitle';
import { AppText } from '@/shared/presentation/components/native/AppText';
import { useTagIncome } from './core/use-tag-income';
import { TagIncomeCard } from './TagIncomeCard';

export const TagIncomeList = () => {
  const { t } = useTranslation();
  const result = useTagIncome();

  if (!result.isSuccess || result.value.length === 0) {
    return (
      <>
        <AppListSectionTitle title={t(tk.insights.income.tagBreakdown.title)} />
        <AppFieldSection>
          <AppText variant="default">{t(tk.insights.income.tagBreakdown.empty)}</AppText>
        </AppFieldSection>
      </>
    );
  }

  return (
    <>
      <AppListSectionTitle title={t(tk.insights.income.tagBreakdown.title)} />
      <AppFieldSection>
        <AppListForEach data={result.value} keyExtractor={(item) => item.tag}>
          {({ item }) => <TagIncomeCard item={item} />}
        </AppListForEach>
      </AppFieldSection>
    </>
  );
};
