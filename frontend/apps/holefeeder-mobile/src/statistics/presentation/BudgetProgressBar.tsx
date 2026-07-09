import { Money } from '@holefeeder/shared/core';
import { StyleSheet, View } from 'react-native';
import { useStyles } from '@/shared/theme/core/use-styles';
import { Theme } from '@/types/theme/theme';

type BudgetProgressBarProps = {
  spentAmount: Money;
  budgetAmount: Money;
  color: string;
};

const createStyles = (theme: Theme) =>
  StyleSheet.create({
    track: {
      height: 6,
      borderRadius: 3,
      backgroundColor: theme.colors.separator,
      overflow: 'hidden',
      marginTop: 4,
    },
    fill: {
      height: '100%',
      borderRadius: 3,
    },
    fillOverBudget: {
      backgroundColor: theme.colors.negative,
    },
  });

export const BudgetProgressBar = ({ spentAmount, budgetAmount, color }: BudgetProgressBarProps) => {
  const styles = useStyles(createStyles);

  if (Money.toCents(budgetAmount) === 0) {
    return null;
  }

  const ratio = Money.toCents(spentAmount) / Money.toCents(budgetAmount);
  const fillWidth = `${Math.min(ratio, 1) * 100}%` as `${number}%`;
  const isOver = ratio > 1;

  return (
    <View style={styles.track}>
      <View style={[styles.fill, { width: fillWidth }, isOver ? styles.fillOverBudget : { backgroundColor: color }]} />
    </View>
  );
};
