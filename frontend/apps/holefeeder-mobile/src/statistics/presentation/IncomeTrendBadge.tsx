import { LocalFormatter } from '@holefeeder/shared/core';
import { useTranslation } from 'react-i18next';
import { tk } from '@/i18n/translations';
import { TrendBadge } from '@/shared/presentation/components/TrendBadge';
import { AppIconMap } from '@/shared/presentation/core/app-icon-map';
import { useLocaleFormatter } from '@/shared/presentation/core/use-local-formatter';
import { ComputedSummary } from '@/summary/core/watch-summary/watch-summary-use-case';

export const IncomeTrendBadge = ({ variation }: { variation: ComputedSummary['incomeVariation'] }) => {
  const { t } = useTranslation();
  const { currentLocale, currencyCode } = useLocaleFormatter();

  return (
    <TrendBadge
      text={t(tk.dashboard.largeHeader.variation, { variation: LocalFormatter.currency(variation.amount, currentLocale, currencyCode) })}
      tone={variation.isBelow ? 'negative' : 'positive'}
      icon={variation.isBelow ? AppIconMap.trendDown : AppIconMap.trendUp}
    />
  );
};
