import { Logger, Money, Result, withDate } from '@holefeeder/shared/core';
import { BillsDueEntry, BillsDueTotal } from '@/dashboard/core/publish-bills-due-widget/bills-due-summary';
import { BillsDueWidgetPublisher, BillsDueWidgetPublisherErrors } from '@/dashboard/core/publish-bills-due-widget/bills-due-widget-publisher';
import BillsDueWidget, { BillsDueAmountProps, BillsDueTotalProps, BillsDueWidgetProps } from '@/dashboard/widgets/BillsDueWidget';

export type BillsDueWidgetDisplay = {
  formatAmount: (amount: Money) => string;
  labels: { title: string; pastDue: string; dueToday: string; openApp: string };
  logo?: string;
};

const logger = Logger.create('BillsDueWidgetPublisherWithExpoWidgets');

export const BillsDueWidgetPublisherWithExpoWidgets = (display: BillsDueWidgetDisplay): BillsDueWidgetPublisher => {
  const amountsProps = (total: BillsDueTotal): BillsDueAmountProps[] => {
    const amounts: BillsDueAmountProps[] = [];
    if (total.expenses > 0) amounts.push({ direction: 'down', text: display.formatAmount(total.expenses) });
    if (total.gains > 0) amounts.push({ direction: 'up', text: display.formatAmount(total.gains) });
    return amounts.length > 0 ? amounts : [{ direction: 'none', text: display.formatAmount(Money.ZERO) }];
  };

  const totalProps = (total: BillsDueTotal, label: string): BillsDueTotalProps => ({
    count: total.count,
    label: label,
    amounts: amountsProps(total),
  });

  const entryProps = (entry: BillsDueEntry): BillsDueWidgetProps => ({
    state: 'ready',
    logo: display.logo,
    title: display.labels.title,
    pastDue: totalProps(entry.summary.pastDue, display.labels.pastDue),
    dueToday: totalProps(entry.summary.dueToday, display.labels.dueToday),
  });

  const update = (apply: () => void): Result<void> => {
    try {
      apply();
      return Result.success();
    } catch (error) {
      if (error instanceof Error) {
        logger.error(`${BillsDueWidgetPublisherErrors.publishFailed}: `, error.message);
      }
      return Result.failure([BillsDueWidgetPublisherErrors.publishFailed]);
    }
  };

  const publish = async (entries: BillsDueEntry[]): Promise<Result<void>> =>
    update(() => BillsDueWidget.updateTimeline(entries.map((entry) => ({ date: withDate(entry.date).toDate(), props: entryProps(entry) }))));

  const clear = async (): Promise<Result<void>> =>
    update(() => BillsDueWidget.updateSnapshot({ state: 'signed-out', logo: display.logo, message: display.labels.openApp }));

  return {
    publish: publish,
    clear: clear,
  };
};
