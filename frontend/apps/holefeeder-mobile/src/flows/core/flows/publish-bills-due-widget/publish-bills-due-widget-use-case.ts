import { DateOnly, Money, Result, withDate } from '@holefeeder/shared/core';
import { UpcomingFlow } from '@/flows/core/flows/upcoming-flow';
import { BillsDueEntry, BillsDueTotal } from './bills-due-summary';
import { BillsDueWidgetPublisher } from './bills-due-widget-publisher';

const MAX_ENTRIES = 64;

const totalOf = (flows: UpcomingFlow[]): BillsDueTotal => ({
  count: flows.length,
  amount: Money.sum(...flows.map((flow) => flow.amount)),
});

export const PublishBillsDueWidgetUseCase = (publisher: BillsDueWidgetPublisher) => {
  const execute = async (flows: UpcomingFlow[], today: DateOnly): Promise<Result<void>> => {
    const entryOn = (date: DateOnly): BillsDueEntry => ({
      date: date,
      summary: {
        pastDue: totalOf(flows.filter((flow) => flow.date < date)),
        dueToday: totalOf(flows.filter((flow) => flow.date === date)),
      },
    });
    const futureDates = flows.flatMap((flow) => [flow.date, withDate(flow.date).addDays(1).toDateOnly()]).filter((date) => date > today);

    return await publisher.publish([today, ...[...new Set(futureDates)].sort()].slice(0, MAX_ENTRIES).map(entryOn));
  };

  const clear = async (): Promise<Result<void>> => await publisher.clear();

  return {
    execute: execute,
    clear: clear,
  };
};
