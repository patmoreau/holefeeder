import { useTranslation } from 'react-i18next';
import { tk } from '@/i18n/translations';
import { AppFieldSection } from '@/shared/presentation/components/native/AppFieldSection';
import { AppListForEach } from '@/shared/presentation/components/native/AppListForEach';
import { AppText } from '@/shared/presentation/components/native/AppText';
import { CategorySpendingCard } from './CategorySpendingCard';
import { useCategorySpending } from './core/use-category-spending';

export const CategorySpendingList = () => {
  const { t } = useTranslation();
  const result = useCategorySpending();

  if (!result.isSuccess || result.value.length === 0) {
    return (
      <AppFieldSection title={t(tk.insights.categoryBreakdown.title)}>
        <AppText variant="default">{t(tk.insights.categoryBreakdown.empty)}</AppText>
      </AppFieldSection>
    );
  }

  return (
    <AppFieldSection title={t(tk.insights.categoryBreakdown.title)}>
      <AppListForEach>
        {result.value.map((item) => (
          <CategorySpendingCard key={item.categoryId} item={item} />
        ))}
      </AppListForEach>
    </AppFieldSection>
  );
};
