import { DateOnly, Money, Result, withDate } from '@holefeeder/shared/core';
import { projectedNetFlow } from '@/dashboard/core/projected-net-flow/projected-net-flow';
import { UpcomingFlow } from '@/flows/core/flows/upcoming-flow';
import { CategoryType, CategoryTypes } from '@/shared/core/category-type';
import { ComputedSummary } from '@/summary/core/watch-summary/watch-summary-use-case';
import { BillsDueEntry, BillsDueTotal } from './bills-due-summary';
import { BillsDueWidgetPublisher } from './bills-due-widget-publisher';

const MAX_ENTRIES = 64;

const sumOf = (flows: UpcomingFlow[], type: CategoryType): Money =>
  Money.sum(...flows.filter((flow) => flow.categoryType === type).map((flow) => flow.amount));

const totalOf = (flows: UpcomingFlow[]): BillsDueTotal => ({
  count: flows.length,
  expenses: sumOf(flows, CategoryTypes.expense),
  gains: sumOf(flows, CategoryTypes.gain),
});

export const PublishBillsDueWidgetUseCase = (publisher: BillsDueWidgetPublisher) => {
  const execute = async (flows: UpcomingFlow[], summary: ComputedSummary, today: DateOnly): Promise<Result<void>> => {
    const entryOn = (date: DateOnly): BillsDueEntry => ({
      date: date,
      summary: {
        pastDue: totalOf(flows.filter((flow) => flow.date < date)),
        dueToday: totalOf(flows.filter((flow) => flow.date === date)),
      },
    });
    const futureDates = flows.flatMap((flow) => [flow.date, withDate(flow.date).addDays(1).toDateOnly()]).filter((date) => date > today);

    return await publisher.publish([today, ...[...new Set(futureDates)].sort()].slice(0, MAX_ENTRIES).map(entryOn), {
      spending: summary.currentSpending,
      projected: projectedNetFlow(summary, flows),
    });
  };

  const clear = async (): Promise<Result<void>> => await publisher.clear();

  return {
    execute: execute,
    clear: clear,
  };
};
