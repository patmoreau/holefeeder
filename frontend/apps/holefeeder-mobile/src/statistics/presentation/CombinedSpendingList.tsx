import { type ComponentProps } from 'react';
import { useTranslation } from 'react-i18next';
import { tk } from '@/i18n/translations';
import { AppFieldSection } from '@/shared/presentation/components/native/AppFieldSection';
import { AppListForEach } from '@/shared/presentation/components/native/AppListForEach';
import { AppListSectionTitle } from '@/shared/presentation/components/native/AppListSectionTitle';
import { AppText } from '@/shared/presentation/components/native/AppText';
import { CategorySpendingCard } from './CategorySpendingCard';
import { CategoryTagSpendingCard } from './CategoryTagSpendingCard';
import { useCombinedSpending } from './core/use-combined-spending';

type CombinedSpendingRow =
  | { key: string; kind: 'category'; item: ComponentProps<typeof CategorySpendingCard>['item'] }
  | { key: string; kind: 'tag'; item: ComponentProps<typeof CategoryTagSpendingCard>['item'] };

export const CombinedSpendingList = () => {
  const { t } = useTranslation();
  const result = useCombinedSpending();

  if (!result.isSuccess || result.value.length === 0) {
    return (
      <>
        <AppListSectionTitle title={t(tk.insights.combinedBreakdown.title)} />
        <AppFieldSection>
          <AppText variant="default">{t(tk.insights.combinedBreakdown.empty)}</AppText>
        </AppFieldSection>
      </>
    );
  }

  const rows = result.value.flatMap((item): CombinedSpendingRow[] => [
    { key: item.category.categoryId, kind: 'category', item: item.category },
    ...item.tags.map((tag): CombinedSpendingRow => ({ key: `${item.category.categoryId}-${tag.tag}`, kind: 'tag', item: tag })),
  ]);

  return (
    <>
      <AppListSectionTitle title={t(tk.insights.combinedBreakdown.title)} />
      <AppFieldSection>
        <AppListForEach data={rows} keyExtractor={(row) => row.key}>
          {({ item: row }) =>
            row.kind === 'category' ? <CategorySpendingCard item={row.item} /> : <CategoryTagSpendingCard item={row.item} />
          }
        </AppListForEach>
      </AppFieldSection>
    </>
  );
};
