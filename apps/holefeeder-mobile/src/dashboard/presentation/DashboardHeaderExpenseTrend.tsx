import { LocalFormatter } from '@holefeeder/shared/core';
import { useTranslation } from 'react-i18next';
import { DashboardComputedSummary } from '@/dashboard/core/watch-summary/watch-summary-use-case';
import { tk } from '@/i18n/translations';
import { AppColumn } from '@/shared/presentation/components/native/AppColumn';
import { AppIcon } from '@/shared/presentation/components/native/AppIcon';
import { AppRow } from '@/shared/presentation/components/native/AppRow';
import { AppText } from '@/shared/presentation/components/native/AppText';
import { AppIconMap } from '@/shared/presentation/core/app-icon-map';
import { useLocaleFormatter } from '@/shared/presentation/core/use-local-formatter';
import { useTheme } from '@/shared/theme/core/use-theme';
import { borderRadius } from '@/types/theme/design-tokens';

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

  const variation = summary.variation;
  const variationText =
    variant === 'amount'
      ? t(tk.dashboard.largeHeader.variation, { variation: LocalFormatter.currency(variation.amount, currentLocale, currencyCode) })
      : LocalFormatter.percentage(variation.percentage);
  const variationColor = variation.isOver ? theme.colors.negative : theme.colors.positive;
  const variationBackgroundColor = variation.isOver ? theme.colors.negativeBackground : theme.colors.positiveBackground;
  const variationIcon = variation.isOver ? AppIconMap.trendUp : AppIconMap.trendDown;

  return (
    <AppColumn
      style={{
        borderRadius: borderRadius.xl,
        backgroundColor: variationBackgroundColor + '80',
        borderColor: variationBackgroundColor + '90',
      }}
    >
      <AppRow spacing={8} style={{ padding: 8 }} alignment={'center'}>
        <AppIcon name={variationIcon} color={variationColor} size={14} />
        <AppText textStyle={{ color: variationColor }}>{variationText}</AppText>
      </AppRow>
    </AppColumn>
  );
};
