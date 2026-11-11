import { LocalFormatter } from '@holefeeder/shared/core';
import { useMemo } from 'react';
import { useTranslation } from 'react-i18next';
import { usePublishBillsDueWidget } from '@/dashboard/presentation/bills-due-widget/use-publish-bills-due-widget';
import { BillsDueWidgetPublisherWithExpoWidgets } from '@/dashboard/widgets/bills-due-widget-publisher-with-expo-widgets';
import { tk } from '@/i18n/translations';
import { useLocaleFormatter } from '@/shared/presentation/core/use-local-formatter';

export const BillsDueWidgetPublisher = ({ logo }: { logo?: string }) => {
  const { t } = useTranslation();
  const { currentLocale, currencyCode } = useLocaleFormatter();
  const publisher = useMemo(
    () =>
      BillsDueWidgetPublisherWithExpoWidgets({
        formatAmount: (amount: number) => LocalFormatter.currency(amount, currentLocale, currencyCode),
        labels: {
          pastDue: t(tk.billsDueWidget.pastDue),
          dueToday: t(tk.billsDueWidget.dueToday),
          spending: t(tk.billsDueWidget.spending),
          projected: t(tk.billsDueWidget.projected),
          openApp: t(tk.billsDueWidget.openApp),
        },
        logo: logo,
      }),
    [t, currentLocale, currencyCode, logo]
  );

  usePublishBillsDueWidget(publisher);
  return null;
};
