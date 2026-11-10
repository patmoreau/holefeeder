import { Logger, Money, Result, withDate } from '@holefeeder/shared/core';
import { BillsDueEntry, BillsDueTotal } from '@/flows/core/flows/publish-bills-due-widget/bills-due-summary';
import { BillsDueWidgetPublisher, BillsDueWidgetPublisherErrors } from '@/flows/core/flows/publish-bills-due-widget/bills-due-widget-publisher';
import BillsDueWidget, { BillsDueTotalProps, BillsDueWidgetProps } from '@/flows/widgets/BillsDueWidget';

export type BillsDueWidgetDisplay = {
  formatAmount: (amount: Money) => string;
  labels: { pastDue: string; dueToday: string; openApp: string };
};

const logger = Logger.create('BillsDueWidgetPublisherWithExpoWidgets');

export const BillsDueWidgetPublisherWithExpoWidgets = (display: BillsDueWidgetDisplay): BillsDueWidgetPublisher => {
  const totalProps = (total: BillsDueTotal, label: string): BillsDueTotalProps => ({
    count: total.count,
    amount: display.formatAmount(total.amount),
    label: label,
  });

  const entryProps = (entry: BillsDueEntry): BillsDueWidgetProps => ({
    state: 'ready',
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
    update(() => BillsDueWidget.updateSnapshot({ state: 'signed-out', message: display.labels.openApp }));

  return {
    publish: publish,
    clear: clear,
  };
};
