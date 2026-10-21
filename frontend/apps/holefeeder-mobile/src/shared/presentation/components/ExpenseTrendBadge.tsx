import { LocalFormatter } from '@holefeeder/shared/core';
import { useTranslation } from 'react-i18next';
import { tk } from '@/i18n/translations';
import { TrendBadge } from '@/shared/presentation/components/TrendBadge';
import { AppIconMap } from '@/shared/presentation/core/app-icon-map';
import { useLocaleFormatter } from '@/shared/presentation/core/use-local-formatter';

export type ExpenseTrend = {
  amount: number;
  percentage: number;
  isOver: boolean;
};

export const ExpenseTrendBadge = ({ variation, variant = 'amount' }: { variation: ExpenseTrend; variant?: 'amount' | 'percentage' }) => {
  const { t } = useTranslation();
  const { currentLocale, currencyCode } = useLocaleFormatter();

  const variationText =
    variant === 'amount'
      ? t(tk.dashboard.largeHeader.variation, { variation: LocalFormatter.currency(variation.amount, currentLocale, currencyCode) })
      : LocalFormatter.percentage(variation.percentage);

  return (
    <TrendBadge
      text={variationText}
      tone={variation.isOver ? 'negative' : 'positive'}
      icon={variation.isOver ? AppIconMap.trendUp : AppIconMap.trendDown}
    />
  );
};
