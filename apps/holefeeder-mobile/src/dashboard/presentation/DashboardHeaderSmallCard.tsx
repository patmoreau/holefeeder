import { useTranslation } from 'react-i18next';
import { View } from 'react-native';
import { DashboardComputedSummary } from '@/dashboard/core/watch-summary/watch-summary-use-case';
import { DashboardHeaderExpenseTrend } from '@/dashboard/presentation/DashboardHeaderExpenseTrend';
import { UpcomingFlow } from '@/flows/core/flows/upcoming-flow';
import { tk } from '@/i18n/translations';
import { AppText } from '@/shared/presentation/components/AppText';
import { useLocaleFormatter } from '@/shared/presentation/core/use-local-formatter';
import { useStyles } from '@/shared/theme/core/use-styles';
import { spacing } from '@/types/theme/design-tokens';
import { Theme } from '@/types/theme/theme';

const createStyles = (theme: Theme) => ({
  container: {
    flexDirection: 'row' as const,
    alignItems: 'center' as const,
    gap: spacing.sm,
  },
  text: {
    color: theme.colors.primaryText,
  },
});

export const DashboardHeaderSmallCard = ({
  summary,
  upcomingFlows = [],
}: {
  summary: DashboardComputedSummary;
  upcomingFlows?: UpcomingFlow[];
}) => {
  const { t } = useTranslation();
  const { formatCurrency } = useLocaleFormatter();
  const styles = useStyles(createStyles);

  return (
    <View style={styles.container}>
      <AppText style={styles.text}>{t(tk.dashboard.smallHeader.spendingTitle)}</AppText>
      <AppText style={styles.text}>{formatCurrency(summary.currentSpending)}</AppText>
      <DashboardHeaderExpenseTrend summary={summary} variant="percentage" />
    </View>
  );
};
