import { LocalFormatter } from '@holefeeder/shared/core';
import { useTranslation } from 'react-i18next';
import { DashboardComputedSummary } from '@/dashboard/core/watch-summary/watch-summary-use-case';
import { tk } from '@/i18n/translations';
import { AppIconMap } from '@/shared/presentation/components/app/app-icon-map';
import { AppHost } from '@/shared/presentation/components/app/AppHost';
import { AppIcon } from '@/shared/presentation/components/app/AppIcon';
import { AppRow } from '@/shared/presentation/components/app/AppRow';
import { AppText } from '@/shared/presentation/components/app/AppText';
import { useLocaleFormatter } from '@/shared/presentation/core/use-local-formatter';
import { useStyles } from '@/shared/theme/core/use-styles';
import { useTheme } from '@/shared/theme/core/use-theme';
import { borderRadius, spacing } from '@/types/theme/design-tokens';

const createStyles = () => ({
  container: {
    gap: spacing.xs,
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
  const { currentLocale, currencyCode } = useLocaleFormatter();
  const { theme } = useTheme();
  const styles = useStyles(createStyles);

  const variation = summary.variation;
  const variationText =
    variant === 'amount'
      ? t(tk.dashboard.largeHeader.variation, { variation: LocalFormatter.currency(variation.amount, currentLocale, currencyCode) })
      : LocalFormatter.percentage(variation.percentage);
  const variationColor = variation.isOver ? theme.colors.negative : theme.colors.positive;
  const variationBackgroundColor = variation.isOver ? theme.colors.negativeBackground : theme.colors.positiveBackground;
  const variationIcon = variation.isOver ? AppIconMap.trendUp : AppIconMap.trendDown;

  return (
    <AppHost
      matchContents
      style={[
        styles.container,
        {
          backgroundColor: variationBackgroundColor + '80',
          borderColor: variationBackgroundColor + '90',
        },
      ]}
    >
      <AppRow spacing={8} style={{ padding: 8 }} alignment={'center'}>
        <AppIcon name={variationIcon} color={variationColor} size={14} />
        <AppText textStyle={{ color: variationColor }}>{variationText}</AppText>
      </AppRow>
    </AppHost>
  );
};
