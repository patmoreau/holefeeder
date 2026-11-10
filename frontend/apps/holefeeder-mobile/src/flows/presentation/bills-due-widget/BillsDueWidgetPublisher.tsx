import { LocalFormatter, Money } from '@holefeeder/shared/core';
import { useMemo } from 'react';
import { useTranslation } from 'react-i18next';
import { usePublishBillsDueWidget } from '@/flows/presentation/bills-due-widget/use-publish-bills-due-widget';
import { BillsDueWidgetPublisherWithExpoWidgets } from '@/flows/widgets/bills-due-widget-publisher-with-expo-widgets';
import { tk } from '@/i18n/translations';
import { useLocaleFormatter } from '@/shared/presentation/core/use-local-formatter';

export const BillsDueWidgetPublisher = () => {
  const { t } = useTranslation();
  const { currentLocale, currencyCode } = useLocaleFormatter();
  const publisher = useMemo(
    () =>
      BillsDueWidgetPublisherWithExpoWidgets({
        formatAmount: (amount: Money) => LocalFormatter.currency(amount, currentLocale, currencyCode),
        labels: { pastDue: t(tk.billsDueWidget.pastDue), dueToday: t(tk.billsDueWidget.dueToday), openApp: t(tk.billsDueWidget.openApp) },
      }),
    [t, currentLocale, currencyCode]
  );

  usePublishBillsDueWidget(publisher);
  return null;
};
