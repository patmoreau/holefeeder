import { LocalFormatter, Money } from '@holefeeder/shared/core';
import { useTranslation } from 'react-i18next';
import { StyleSheet, View } from 'react-native';
import { tk } from '@/i18n/translations';
import { AppText } from '@/shared/presentation/components/native/AppText';
import { useLocaleFormatter } from '@/shared/presentation/core/use-local-formatter';
import { useStyles } from '@/shared/theme/core/use-styles';
import { CategorySpending } from '@/statistics/core/category-spending';
import { Theme } from '@/types/theme/theme';
import { BudgetProgressBar } from './BudgetProgressBar';

type CategorySpendingCardProps = {
  item: CategorySpending;
};

const createStyles = (theme: Theme) =>
  StyleSheet.create({
    container: {
      paddingVertical: 8,
      paddingHorizontal: 16,
    },
    row: {
      flexDirection: 'row',
      alignItems: 'center',
      justifyContent: 'space-between',
    },
    nameRow: {
      flexDirection: 'row',
      alignItems: 'center',
      gap: 8,
      flex: 1,
    },
    colorDot: {
      width: 10,
      height: 10,
      borderRadius: 5,
    },
    categoryName: {
      color: theme.colors.text,
      flex: 1,
    },
    amountNeutral: {
      color: theme.colors.text,
    },
    amountPositive: {
      color: theme.colors.positive,
    },
    amountNegative: {
      color: theme.colors.negative,
    },
  });

export const CategorySpendingCard = ({ item }: CategorySpendingCardProps) => {
  const { t } = useTranslation();
  const styles = useStyles(createStyles);
  const { currentLocale, currencyCode } = useLocaleFormatter();

  const hasBudget = Money.toCents(item.budgetAmount) > 0;
  const isOverBudget = hasBudget && Money.toCents(item.spentAmount) > Money.toCents(item.budgetAmount);
  const isUnderBudget = hasBudget && !isOverBudget;

  const amountStyle = isOverBudget ? styles.amountNegative : isUnderBudget ? styles.amountPositive : styles.amountNeutral;

  return (
    <View style={styles.container}>
      <View style={styles.row}>
        <View style={styles.nameRow}>
          <View style={[styles.colorDot, { backgroundColor: item.color }]} />
          <AppText variant="default" style={styles.categoryName}>
            {item.categoryName}
          </AppText>
        </View>
        <AppText variant="defaultSemiBold" style={amountStyle}>
          {LocalFormatter.currency(item.spentAmount, currentLocale, currencyCode)}
        </AppText>
      </View>
      <BudgetProgressBar spentAmount={item.spentAmount} budgetAmount={item.budgetAmount} color={item.color} />
      {isOverBudget && (
        <AppText variant="footnote" style={styles.amountNegative}>
          {t(tk.insights.categoryBreakdown.overBudget)}
        </AppText>
      )}
    </View>
  );
};
