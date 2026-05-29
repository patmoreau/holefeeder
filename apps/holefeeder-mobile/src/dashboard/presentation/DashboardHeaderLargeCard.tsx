import { LocalFormatter, Money } from '@holefeeder/shared/core';
import { useTranslation } from 'react-i18next';
import { View } from 'react-native';
import { DashboardComputedSummary } from '@/dashboard/core/watch-summary/watch-summary-use-case';
import { DashboardHeaderExpenseTrend } from '@/dashboard/presentation/DashboardHeaderExpenseTrend';
import { CategoryType } from '@/flows/core/categories/category-type';
import { UpcomingFlow } from '@/flows/core/flows/upcoming-flow';
import { tk } from '@/i18n/translations';
import { AppText } from '@/shared/presentation/components/AppText';
import { useLocaleFormatter } from '@/shared/presentation/core/use-local-formatter';
import { useStyles } from '@/shared/theme/core/use-styles';
import { borderRadius, fontWeight, spacing } from '@/types/theme/design-tokens';
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
  pill: {
    borderRadius: borderRadius.full,
    paddingHorizontal: spacing.sm,
    paddingVertical: spacing.xs,
  },
  positivePill: {
    backgroundColor: theme.colors.positiveBackground,
  },
  negativePill: {
    backgroundColor: theme.colors.negativeBackground,
  },
  positiveText: {
    color: theme.colors.positive,
    fontWeight: fontWeight.semiBold,
  },
  negativeText: {
    color: theme.colors.negative,
    fontWeight: fontWeight.semiBold,
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
  const { currentLocale, currencyCode } = useLocaleFormatter();
  const styles = useStyles(createStyles);

  const netFlow = summary.netFlow;
  const netFlowText = netFlow.isOver
    ? `+ ${LocalFormatter.currency(netFlow.amount, currentLocale, currencyCode)}`
    : `- ${LocalFormatter.currency(Math.abs(netFlow.amount), currentLocale, currencyCode)}`;
  const netFlowPositive = netFlow.isOver;

  const upcomingVariation = upcomingFlows.reduce((acc, flow) => {
    return acc + (Money.toCents(flow.amount) / 100) * CategoryType.multiplier[flow.categoryType];
  }, 0);

  const baseNetFlow = summary.netFlow.isOver ? summary.netFlow.amount : -summary.netFlow.amount;
  const projectedNetFlowTotal = baseNetFlow + upcomingVariation;
  const projectedIsOver = projectedNetFlowTotal >= 0;
  const projectedNetFlowAmount = Math.abs(projectedNetFlowTotal);

  const projectedNetFlowText = projectedIsOver
    ? `+ ${LocalFormatter.currency(projectedNetFlowAmount, currentLocale, currencyCode)}`
    : `- ${LocalFormatter.currency(projectedNetFlowAmount, currentLocale, currencyCode)}`;

  return (
    <>
      <AppText variant={'subtitle'} style={styles.textColor}>
        {t(tk.dashboard.largeHeader.spendingTitle)}
      </AppText>
      <AppText variant={'display'} style={styles.largeTitle}>
        {LocalFormatter.currency(summary.currentSpending, currentLocale, currencyCode)}
      </AppText>
      <DashboardHeaderExpenseTrend summary={summary} variant="amount" />
      <View style={styles.divider} />
      <View style={{ flexDirection: 'row', flex: 1, justifyContent: 'space-between' }}>
        <View style={{ flex: 1, alignItems: 'center' }}>
          <AppText variant={'subtitle'} style={styles.subtitle}>
            {t(tk.dashboard.largeHeader.netFlow)}
          </AppText>
          <View style={[styles.pill, netFlowPositive ? styles.positivePill : styles.negativePill]}>
            <AppText style={netFlowPositive ? styles.positiveText : styles.negativeText}>{netFlowText}</AppText>
          </View>
        </View>
        <View style={{ flex: 1, alignItems: 'center' }}>
          <AppText variant={'subtitle'} style={styles.subtitle}>
            {t(tk.accountCard.projected)}
          </AppText>
          <View style={[styles.pill, projectedIsOver ? styles.positivePill : styles.negativePill]}>
            <AppText style={projectedIsOver ? styles.positiveText : styles.negativeText}>{projectedNetFlowText}</AppText>
          </View>
        </View>
      </View>
    </>
  );
};
