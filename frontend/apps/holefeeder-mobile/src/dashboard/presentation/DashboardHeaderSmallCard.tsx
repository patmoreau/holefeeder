import { LocalFormatter } from '@holefeeder/shared/core';
import { useTranslation } from 'react-i18next';
import { View } from 'react-native';
import { tk } from '@/i18n/translations';
import { AppText } from '@/shared/presentation/components/AppText';
import { useLocaleFormatter } from '@/shared/presentation/core/use-local-formatter';
import { useStyles } from '@/shared/theme/core/use-styles';
import { ComputedSummary } from '@/summary/core/watch-summary/watch-summary-use-case';
import { fontWeight, spacing } from '@/types/theme/design-tokens';
import { Theme } from '@/types/theme/theme';

const createStyles = (theme: Theme) => ({
  row: {
    flexDirection: 'row' as const,
    alignItems: 'center' as const,
    gap: spacing.sm,
    flexWrap: 'wrap' as const,
  },
  label: {
    color: theme.colors.primaryText,
  },
  amount: {
    color: theme.colors.primaryText,
    fontWeight: fontWeight.semiBold,
  },
});

export const DashboardHeaderSmallCard = ({ summary }: { summary: ComputedSummary }) => {
  const { t } = useTranslation();
  const { currentLocale, currencyCode } = useLocaleFormatter();
  const styles = useStyles(createStyles);

  return (
    <View style={styles.row} testID="dashboard-header-small">
      <AppText variant={'subtitle'} style={styles.label}>
        {t(tk.dashboard.smallHeader.spendingTitle)}
      </AppText>
      <AppText variant={'title'} style={styles.amount}>
        {LocalFormatter.currency(summary.currentSpending, currentLocale, currencyCode)}
      </AppText>
    </View>
  );
};
