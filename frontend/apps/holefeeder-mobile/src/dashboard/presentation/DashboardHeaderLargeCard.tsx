import { LocalFormatter, Money } from '@holefeeder/shared/core';
import { useTranslation } from 'react-i18next';
import { DashboardComputedSummary } from '@/dashboard/core/watch-summary/watch-summary-use-case';
import { DashboardHeaderExpenseTrend } from '@/dashboard/presentation/DashboardHeaderExpenseTrend';
import { UpcomingFlow } from '@/flows/core/flows/upcoming-flow';
import { tk } from '@/i18n/translations';
import { CategoryType } from '@/shared/core/category-type';
import { AppColumn } from '@/shared/presentation/components/native/AppColumn';
import { AppDivider } from '@/shared/presentation/components/native/AppDivider';
import { AppNative } from '@/shared/presentation/components/native/AppNative';
import { AppRow } from '@/shared/presentation/components/native/AppRow';
import { AppSpacer } from '@/shared/presentation/components/native/AppSpacer';
import { AppText } from '@/shared/presentation/components/native/AppText';
import { useLocaleFormatter } from '@/shared/presentation/core/use-local-formatter';
import { useStyles } from '@/shared/theme/core/use-styles';
import { useTheme } from '@/shared/theme/core/use-theme';
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
  const { theme } = useTheme();
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
    <AppNative style={{ flex: 1 }}>
      <AppColumn spacing={8} alignment={'start'} style={{ paddingBottom: spacing.lg }}>
        <AppRow>
          <AppText variant={'subtitle'} textStyle={styles.textColor}>
            {t(tk.dashboard.largeHeader.spendingTitle)}
          </AppText>
          <AppSpacer />
        </AppRow>
        <AppRow>
          <AppText variant={'display'} textStyle={styles.largeTitle}>
            {LocalFormatter.currency(summary.currentSpending, currentLocale, currencyCode)}
          </AppText>
        </AppRow>
        <AppRow>
          <DashboardHeaderExpenseTrend summary={summary} variant="amount" />
          <AppSpacer />
        </AppRow>
        <AppDivider />
        <AppRow>
          <AppSpacer />
          <AppColumn alignment={'center'} spacing={4}>
            <AppText variant={'subtitle'} style={styles.subtitle}>
              {t(tk.dashboard.largeHeader.netFlow)}
            </AppText>
            <AppColumn
              style={{
                paddingHorizontal: spacing.sm,
                paddingVertical: spacing.xs,
                borderRadius: borderRadius.full,
                backgroundColor: projectedIsOver ? theme.colors.positiveBackground : theme.colors.negativeBackground,
              }}
            >
              <AppText textStyle={netFlowPositive ? styles.positiveText : styles.negativeText}>{netFlowText}</AppText>
            </AppColumn>
          </AppColumn>
          <AppSpacer />
          <AppColumn alignment={'center'} spacing={4}>
            <AppText variant={'subtitle'} style={styles.subtitle}>
              {t(tk.accountCard.projected)}
            </AppText>
            <AppColumn
              style={{
                paddingHorizontal: spacing.sm,
                paddingVertical: spacing.xs,
                borderRadius: borderRadius.full,
                backgroundColor: projectedIsOver ? theme.colors.positiveBackground : theme.colors.negativeBackground,
              }}
            >
              <AppText textStyle={projectedIsOver ? styles.positiveText : styles.negativeText}>{projectedNetFlowText}</AppText>
            </AppColumn>
          </AppColumn>
          <AppSpacer />
        </AppRow>
        <AppSpacer />
      </AppColumn>
    </AppNative>
  );
};
