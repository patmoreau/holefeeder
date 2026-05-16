import { useTranslation } from 'react-i18next';
import { View } from 'react-native';
import { DashboardComputedSummary } from '@/dashboard/core/watch-summary/watch-summary-use-case';
import { tk } from '@/i18n/translations';
import { AppText } from '@/shared/presentation/components/AppText';
import { IconSymbol } from '@/shared/presentation/components/IconSymbol';
import { useLocaleFormatter } from '@/shared/presentation/core/use-local-formatter';
import { AppIcons } from '@/shared/presentation/icons';
import { useStyles } from '@/shared/theme/core/use-styles';
import { useTheme } from '@/shared/theme/core/use-theme';
import { borderRadius, spacing } from '@/types/theme/design-tokens';

const createStyles = () => ({
  container: {
    alignSelf: 'flex-start' as const,
    flexDirection: 'row' as const,
    alignItems: 'center' as const,
    gap: spacing.xs,
    paddingHorizontal: spacing.sm,
    paddingVertical: spacing.xs,
    borderRadius: borderRadius.xl,
    borderWidth: 1,
  },
});

export const DashboardHeaderExpenseTrend = ({
  summary,
  variant = 'amount',
}: {
  summary: DashboardComputedSummary;
  variant?: 'amount' | 'percentage';
}) => {
  const { t } = useTranslation();
  const { formatCurrency, formatPercentage } = useLocaleFormatter();
  const { theme } = useTheme();
  const styles = useStyles(createStyles);

  const variation = summary.variation;
  const variationText =
    variant === 'amount'
      ? t(tk.dashboard.largeHeader.variation, { variation: formatCurrency(variation.amount) })
      : formatPercentage(variation.percentage);
  const variationColor = variation.isOver ? theme.colors.negative : theme.colors.positive;
  const variationBackgroundColor = variation.isOver ? theme.colors.negativeBackground : theme.colors.positiveBackground;
  const variationIcon = variation.isOver ? AppIcons.trendUp : AppIcons.trendDown;

  return (
    <View
      style={[
        styles.container,
        {
          backgroundColor: variationBackgroundColor + '80',
          borderColor: variationBackgroundColor + '90',
        },
      ]}
    >
      <IconSymbol name={variationIcon} color={variationColor} size={14} />
      <AppText style={{ color: variationColor }}>{variationText}</AppText>
    </View>
  );
};
