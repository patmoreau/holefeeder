import { useTranslation } from 'react-i18next';
import { View } from 'react-native';
import { DashboardComputedSummary } from '@/dashboard/core/watch-summary/watch-summary-use-case';
import { DashboardHeaderExpenseTrend } from '@/dashboard/presentation/DashboardHeaderExpenseTrend';
import { CategoryType } from '@/flows/core/categories/category-type';
import { UpcomingFlow } from '@/flows/core/flows/upcoming-flow';
import { tk } from '@/i18n/translations';
import { Money } from '@/shared/core/money';
import { AppText } from '@/shared/presentation/components/AppText';
import { useLocaleFormatter } from '@/shared/presentation/core/use-local-formatter';
import { useStyles } from '@/shared/theme/core/use-styles';
import { useTheme } from '@/shared/theme/core/use-theme';
import { fontWeight, spacing } from '@/types/theme/design-tokens';
import { Theme } from '@/types/theme/theme';

const createStyles = (theme: Theme) => ({
  textColor: {
    color: theme.colors.primaryText,
  },
  largeTitle: {
    fontWeight: fontWeight.bold,
    color: theme.colors.primaryText,
    marginBottom: spacing.xs,
  },
  divider: {
    height: 1,
    backgroundColor: theme.colors.primaryText,
    opacity: 0.2,
    marginVertical: spacing.lg,
  },
  subtitle: {
    color: theme.colors.primaryText,
    opacity: 0.5,
    marginBottom: spacing.xs,
  },
});

export const DashboardHeaderLargeCard = ({
  summary,
  upcomingFlows = [],
}: {
  summary: DashboardComputedSummary;
  upcomingFlows?: UpcomingFlow[];
}) => {
  const { t } = useTranslation();
  const { formatCurrency } = useLocaleFormatter();
  const { theme } = useTheme();
  const styles = useStyles(createStyles);

  const netFlow = summary.netFlow;
  const netFlowText = netFlow.isOver ? `+ ${formatCurrency(netFlow.amount)}` : `- ${formatCurrency(Math.abs(netFlow.amount))}`;
  const netFlowColor = netFlow.isOver ? theme.colors.primaryText : theme.colors.secondaryText;

  const upcomingVariation = upcomingFlows.reduce((acc, flow) => {
    return acc + (Money.toCents(flow.amount) / 100) * CategoryType.multiplier[flow.categoryType];
  }, 0);

  const baseNetFlow = summary.netFlow.isOver ? summary.netFlow.amount : -summary.netFlow.amount;
  const projectedNetFlowTotal = baseNetFlow + upcomingVariation;
  const projectedIsOver = projectedNetFlowTotal >= 0;
  const projectedNetFlowAmount = Math.abs(projectedNetFlowTotal);

  const projectedNetFlowText = projectedIsOver ? `+ ${formatCurrency(projectedNetFlowAmount)}` : `- ${formatCurrency(projectedNetFlowAmount)}`;
  const projectedNetFlowColor = projectedIsOver ? theme.colors.primaryText : theme.colors.negative;

  return (
    <>
      <AppText variant={'subtitle'} style={styles.textColor}>
        {t(tk.dashboard.largeHeader.spendingTitle)}
      </AppText>
      <AppText variant={'largeTitle'} style={styles.largeTitle}>
        {formatCurrency(summary.currentSpending)}
      </AppText>
      <DashboardHeaderExpenseTrend summary={summary} variant="amount" />
      <View style={styles.divider} />
      <View style={{ flexDirection: 'row', flex: 1, justifyContent: 'space-between' }}>
        <View style={{ flex: 1, alignItems: 'center' }}>
          <AppText variant={'subtitle'} style={styles.subtitle}>
            {t(tk.dashboard.largeHeader.netFlow)}
          </AppText>
          <AppText style={[{ color: netFlowColor }]}>{netFlowText}</AppText>
        </View>
        <View style={{ flex: 1, alignItems: 'center' }}>
          <AppText variant={'subtitle'} style={styles.subtitle}>
            {t(tk.accountCard.projected)}
          </AppText>
          <AppText style={[{ color: projectedNetFlowColor }]}>{projectedNetFlowText}</AppText>
        </View>
      </View>
    </>
  );
};
