import { LocalFormatter } from '@holefeeder/shared/core';
import { useTranslation } from 'react-i18next';
import { DashboardComputedSummary } from '@/dashboard/core/watch-summary/watch-summary-use-case';
import { UpcomingFlow } from '@/flows/core/flows/upcoming-flow';
import { tk } from '@/i18n/translations';
import { ExpenseTrendBadge } from '@/shared/presentation/components/ExpenseTrendBadge';
import { AppRow } from '@/shared/presentation/components/native/AppRow';
import { AppText } from '@/shared/presentation/components/native/AppText';
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
  const { currentLocale, currencyCode } = useLocaleFormatter();
  const styles = useStyles(createStyles);

  return (
    <AppRow style={{ backgroundColor: 'blue', padding: spacing.lg, paddingBottom: spacing.sm }}>
      <AppText textStyle={styles.text}>{t(tk.dashboard.smallHeader.spendingTitle)}</AppText>
      <AppText textStyle={styles.text}>{LocalFormatter.currency(summary.currentSpending, currentLocale, currencyCode)}</AppText>
      <ExpenseTrendBadge variation={summary.variation} variant="percentage" />
    </AppRow>
  );
};
