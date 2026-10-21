import { LocalFormatter } from '@holefeeder/shared/core';
import { useTranslation } from 'react-i18next';
import { tk } from '@/i18n/translations';
import { TrendBadge } from '@/shared/presentation/components/TrendBadge';
import { AppIconMap } from '@/shared/presentation/core/app-icon-map';
import { useLocaleFormatter } from '@/shared/presentation/core/use-local-formatter';
import { NetBalanceKinds, NetBalanceResult } from '@/summary/core/net-balance';

export const NetBalanceBadge = ({ balance }: { balance: NetBalanceResult }) => {
  const { t } = useTranslation();
  const { currentLocale, currencyCode } = useLocaleFormatter();

  switch (balance.kind) {
    case NetBalanceKinds.saved:
      return (
        <TrendBadge
          text={t(tk.insights.net.saved, {
            percentage: new Intl.NumberFormat(currentLocale, { style: 'percent' }).format(balance.percentage / 100),
          })}
          tone="positive"
          icon={AppIconMap.trendUp}
        />
      );
    case NetBalanceKinds.overspent:
      return (
        <TrendBadge
          text={t(tk.insights.net.overspent, { amount: LocalFormatter.currency(balance.amount, currentLocale, currencyCode) })}
          tone="negative"
          icon={AppIconMap.warning}
        />
      );
    case NetBalanceKinds.brokeEven:
      return <TrendBadge text={t(tk.insights.net.brokeEven)} tone="neutral" icon={AppIconMap.equal} />;
    case NetBalanceKinds.none:
      return null;
  }
};
