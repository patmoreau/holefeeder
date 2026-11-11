import { LocalFormatter } from '@holefeeder/shared/core';
import { useMemo } from 'react';
import { useTranslation } from 'react-i18next';
import { usePublishSpendingWidget } from '@/dashboard/presentation/spending-widget/use-publish-spending-widget';
import { SpendingWidgetPublisherWithExpoWidgets } from '@/dashboard/widgets/spending-widget-publisher-with-expo-widgets';
import { tk } from '@/i18n/translations';
import { useLocaleFormatter } from '@/shared/presentation/core/use-local-formatter';

export const SpendingWidgetPublisher = ({ logo }: { logo?: string }) => {
  const { t } = useTranslation();
  const { currentLocale, currencyCode } = useLocaleFormatter();
  const publisher = useMemo(
    () =>
      SpendingWidgetPublisherWithExpoWidgets({
        formatAmount: (amount: number) => LocalFormatter.currency(amount, currentLocale, currencyCode),
        formatTrend: (amount: string) => t(tk.spendingWidget.versusAverage, { variation: amount }),
        labels: {
          title: t(tk.spendingWidget.title),
          projected: t(tk.spendingWidget.projected),
          openApp: t(tk.spendingWidget.openApp),
        },
        logo: logo,
      }),
    [t, currentLocale, currencyCode, logo]
  );

  usePublishSpendingWidget(publisher);

  return null;
};
