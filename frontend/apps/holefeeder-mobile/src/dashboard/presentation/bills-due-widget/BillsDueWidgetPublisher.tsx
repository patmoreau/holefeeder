import { LocalFormatter, Money } from '@holefeeder/shared/core';
import { useEffect, useMemo, useState } from 'react';
import { useTranslation } from 'react-i18next';
import { usePublishBillsDueWidget } from '@/dashboard/presentation/bills-due-widget/use-publish-bills-due-widget';
import { BillsDueWidgetPublisherWithExpoWidgets } from '@/dashboard/widgets/bills-due-widget-publisher-with-expo-widgets';
import { installWidgetLogo } from '@/dashboard/widgets/install-widget-logo';
import { tk } from '@/i18n/translations';
import { useLocaleFormatter } from '@/shared/presentation/core/use-local-formatter';

export const BillsDueWidgetPublisher = () => {
  const { t } = useTranslation();
  const { currentLocale, currencyCode } = useLocaleFormatter();
  const [logo, setLogo] = useState<string>();

  useEffect(() => {
    installWidgetLogo().then((result) => {
      if (result.isSuccess) setLogo(result.value);
    });
  }, []);

  const publisher = useMemo(
    () =>
      BillsDueWidgetPublisherWithExpoWidgets({
        formatAmount: (amount: Money) => LocalFormatter.currency(amount, currentLocale, currencyCode),
        labels: {
          title: t(tk.billsDueWidget.title),
          pastDue: t(tk.billsDueWidget.pastDue),
          dueToday: t(tk.billsDueWidget.dueToday),
          openApp: t(tk.billsDueWidget.openApp),
        },
        logo: logo,
      }),
    [t, currentLocale, currencyCode, logo]
  );

  usePublishBillsDueWidget(publisher);
  return null;
};
