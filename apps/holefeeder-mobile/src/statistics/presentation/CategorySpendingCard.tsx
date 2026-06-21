import { LocalFormatter, Money } from '@holefeeder/shared/core';
import { useTranslation } from 'react-i18next';
import { tk } from '@/i18n/translations';
import { AppColumn } from '@/shared/presentation/components/native/AppColumn';
import { AppListItem } from '@/shared/presentation/components/native/AppListItem';
import { AppText } from '@/shared/presentation/components/native/AppText';
import { useLocaleFormatter } from '@/shared/presentation/core/use-local-formatter';
import { useTheme } from '@/shared/theme/core/use-theme';
import { CategorySpending } from '@/statistics/core/category-spending';

type CategorySpendingCardProps = {
  item: CategorySpending;
};

export const CategorySpendingCard = ({ item }: CategorySpendingCardProps) => {
  const { t } = useTranslation();
  const { theme } = useTheme();
  const { currentLocale, currencyCode } = useLocaleFormatter();

  const budgetCents = Money.toCents(item.budgetAmount);
  const spentCents = Money.toCents(item.spentAmount);
  const hasBudget = budgetCents > 0;
  const isOverBudget = hasBudget && spentCents > budgetCents;
  const isUnderBudget = hasBudget && !isOverBudget;
  const ratio = hasBudget ? spentCents / budgetCents : 0;

  return (
    <AppListItem>
      <AppText variant="default">{item.categoryName}</AppText>
      <AppListItem.Trailing>
        <AppColumn alignment="end">
          <AppText
            variant="defaultSemiBold"
            textStyle={{ color: isOverBudget ? theme.colors.negative : isUnderBudget ? theme.colors.positive : undefined }}
          >
            {LocalFormatter.currency(item.spentAmount, currentLocale, currencyCode)}
          </AppText>
          {isOverBudget && (
            <AppText variant="footnote" textStyle={{ color: theme.colors.negative }}>
              {t(tk.insights.categoryBreakdown.overBudget)}
            </AppText>
          )}
          {hasBudget && !isOverBudget && <AppText variant="footnote">{`${Math.round(ratio * 100)}%`}</AppText>}
        </AppColumn>
      </AppListItem.Trailing>
    </AppListItem>
  );
};
