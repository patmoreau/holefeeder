import { LocalFormatter, Money } from '@holefeeder/shared/core';
import { useTranslation } from 'react-i18next';
import { View } from 'react-native';
import { signedAmount } from '@/dashboard/presentation/core/signed-amount';
import { UpcomingFlow } from '@/flows/core/flows/upcoming-flow';
import { tk } from '@/i18n/translations';
import { CategoryType } from '@/shared/core/category-type';
import { AppText } from '@/shared/presentation/components/AppText';
import { ExpenseTrendBadge } from '@/shared/presentation/components/ExpenseTrendBadge';
import { AppNative } from '@/shared/presentation/components/native/AppNative';
import { useLocaleFormatter } from '@/shared/presentation/core/use-local-formatter';
import { useStyles } from '@/shared/theme/core/use-styles';
import { useTheme } from '@/shared/theme/core/use-theme';
import { ComputedSummary } from '@/summary/core/watch-summary/watch-summary-use-case';
import { borderRadius, fontWeight, spacing } from '@/types/theme/design-tokens';
import { Theme } from '@/types/theme/theme';

// Built from React Native views rather than a SwiftUI host so the card reports its own height.
// The header above it sizes to that, which is what keeps the layout correct at every Dynamic
// Type setting — a host only reports its content size with `matchContents`, and that collapses
// the width as well, leaving the divider and pills too narrow.
const createStyles = (theme: Theme) => ({
  column: {
    gap: spacing.sm,
    paddingBottom: spacing.lg,
  },
  textColor: {
    color: theme.colors.primaryText,
  },
  largeTitle: {
    fontWeight: fontWeight.bold,
    color: theme.colors.primaryText,
  },
  subtitle: {
    color: theme.colors.primaryText,
  },
  divider: {
    height: 1,
    backgroundColor: theme.colors.primaryText,
  },
  totals: {
    flexDirection: 'row' as const,
    justifyContent: 'space-evenly' as const,
    // Without this the columns stretch to the row height and the pills balloon with it.
    alignItems: 'flex-start' as const,
  },
  total: {
    alignItems: 'center' as const,
    gap: spacing.xs,
  },
  pill: {
    paddingHorizontal: spacing.sm,
    paddingVertical: spacing.xs,
    borderRadius: borderRadius.full,
    alignSelf: 'center' as const,
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

export const DashboardHeaderLargeCard = ({ summary, upcomingFlows = [] }: { summary: ComputedSummary; upcomingFlows?: UpcomingFlow[] }) => {
  const { t } = useTranslation();
  const { theme } = useTheme();
  const { currentLocale, currencyCode } = useLocaleFormatter();
  const styles = useStyles(createStyles);

  const netFlow = signedAmount(summary.netFlow.isOver, summary.netFlow.amount, currentLocale, currencyCode);

  const upcomingVariation = upcomingFlows.reduce((acc, flow) => {
    return acc + (Money.toCents(flow.amount) / 100) * CategoryType.multiplier[flow.categoryType];
  }, 0);

  const baseNetFlow = summary.netFlow.isOver ? summary.netFlow.amount : -summary.netFlow.amount;
  const projectedNetFlowTotal = baseNetFlow + upcomingVariation;
  const projectedIsOver = projectedNetFlowTotal >= 0;
  const projected = signedAmount(projectedIsOver, projectedNetFlowTotal, currentLocale, currencyCode);

  const pillBackground = { backgroundColor: projectedIsOver ? theme.colors.positiveBackground : theme.colors.negativeBackground };

  return (
    <View style={styles.column} testID="dashboard-header-large">
      <AppText variant={'subtitle'} style={styles.textColor}>
        {t(tk.dashboard.largeHeader.spendingTitle)}
      </AppText>
      <AppText variant={'display'} style={styles.largeTitle}>
        {LocalFormatter.currency(summary.currentSpending, currentLocale, currencyCode)}
      </AppText>
      {/* The badge is shared with a SwiftUI screen, so it stays native and sizes to its content. */}
      <AppNative matchContents>
        <ExpenseTrendBadge variation={summary.variation} variant="amount" />
      </AppNative>
      <View style={styles.divider} />
      <View style={styles.totals}>
        <View style={styles.total}>
          <AppText variant={'subtitle'} style={styles.subtitle}>
            {t(tk.dashboard.largeHeader.netFlow)}
          </AppText>
          <View style={[styles.pill, pillBackground]}>
            <AppText style={netFlow.tone === 'positive' ? styles.positiveText : styles.negativeText}>{netFlow.text}</AppText>
          </View>
        </View>
        <View style={styles.total}>
          <AppText variant={'subtitle'} style={styles.subtitle}>
            {t(tk.accountCard.projected)}
          </AppText>
          <View style={[styles.pill, pillBackground]}>
            <AppText style={projected.tone === 'positive' ? styles.positiveText : styles.negativeText}>{projected.text}</AppText>
          </View>
        </View>
      </View>
    </View>
  );
};
