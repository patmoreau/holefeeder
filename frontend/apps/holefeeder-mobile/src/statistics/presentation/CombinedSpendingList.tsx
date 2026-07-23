import { useTranslation } from 'react-i18next';
import { tk } from '@/i18n/translations';
import { AppFieldSection } from '@/shared/presentation/components/native/AppFieldSection';
import { AppListForEach } from '@/shared/presentation/components/native/AppListForEach';
import { AppText } from '@/shared/presentation/components/native/AppText';
import { CategorySpendingCard } from './CategorySpendingCard';
import { CategoryTagSpendingCard } from './CategoryTagSpendingCard';
import { useCombinedSpending } from './core/use-combined-spending';

export const CombinedSpendingList = () => {
  const { t } = useTranslation();
  const result = useCombinedSpending();

  if (!result.isSuccess || result.value.length === 0) {
    return (
      <AppFieldSection title={t(tk.insights.combinedBreakdown.title)}>
        <AppText variant="default">{t(tk.insights.combinedBreakdown.empty)}</AppText>
      </AppFieldSection>
    );
  }

  return (
    <AppFieldSection title={t(tk.insights.combinedBreakdown.title)}>
      <AppListForEach>
        {result.value.flatMap((item) => [
          <CategorySpendingCard key={item.category.categoryId} item={item.category} />,
          ...item.tags.map((tag) => <CategoryTagSpendingCard key={`${item.category.categoryId}-${tag.tag}`} item={tag} />),
        ])}
      </AppListForEach>
    </AppFieldSection>
  );
};
