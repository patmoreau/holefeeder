import { DateOnly, Money, withDate } from '@holefeeder/shared/core';
import { BillsDueWidgetPublisherInMemory } from '@/dashboard/core/__tests__/bills-due-widget-publisher-in-memory';
import { PublishBillsDueWidgetUseCase } from '@/dashboard/core/publish-bills-due-widget/publish-bills-due-widget-use-case';
import { aUpcomingFlow } from '@/flows/core/flows/__tests__/upcoming-flow-for-test';
import { CategoryTypes } from '@/shared/core/category-type';
import { NO_SUMMARY } from '@/summary/core/watch-summary/watch-summary-use-case';

const today = DateOnly.valid('2026-11-10');
const inDays = (days: number) => withDate(today).addDays(days).toDateOnly();
const total = (count: number, expenses: number, gains = 0) => ({ count: count, expenses: Money.valid(expenses), gains: Money.valid(gains) });

describe('PublishBillsDueWidgetUseCase', () => {
  let publisher: BillsDueWidgetPublisherInMemory;
  let useCase: ReturnType<typeof PublishBillsDueWidgetUseCase>;

  beforeEach(() => {
    publisher = BillsDueWidgetPublisherInMemory();
    useCase = PublishBillsDueWidgetUseCase(publisher);
  });

  it('should publish an empty summary for today when no bills are upcoming', async () => {
    const result = await useCase.execute([], NO_SUMMARY, today);

    expect(result).toBeSuccessWithValue(undefined);
    expect(publisher.published()).toEqual([[{ date: today, summary: { pastDue: total(0, 0), dueToday: total(0, 0) } }]]);
  });

  it('should count a bill due today', async () => {
    await useCase.execute([aUpcomingFlow({ date: today, amount: Money.valid(12.5) })], NO_SUMMARY, today);

    expect(publisher.published()[0][0]).toEqual({ date: today, summary: { pastDue: total(0, 0), dueToday: total(1, 12.5) } });
  });

  it('should count an overdue bill as past due', async () => {
    await useCase.execute([aUpcomingFlow({ date: inDays(-1), amount: Money.valid(20) })], NO_SUMMARY, today);

    expect(publisher.published()[0][0]).toEqual({ date: today, summary: { pastDue: total(1, 20), dueToday: total(0, 0) } });
  });

  it('should publish entries for the day a future bill is due and the day after', async () => {
    await useCase.execute([aUpcomingFlow({ date: inDays(3), amount: Money.valid(10) })], NO_SUMMARY, today);

    expect(publisher.published()).toEqual([
      [
        { date: today, summary: { pastDue: total(0, 0), dueToday: total(0, 0) } },
        { date: inDays(3), summary: { pastDue: total(0, 0), dueToday: total(1, 10) } },
        { date: inDays(4), summary: { pastDue: total(1, 10), dueToday: total(0, 0) } },
      ],
    ]);
  });

  it('should publish one entry per date in date order', async () => {
    await useCase.execute(
      [
        aUpcomingFlow({ date: inDays(2), amount: Money.valid(5) }),
        aUpcomingFlow({ date: inDays(1), amount: Money.valid(3) }),
        aUpcomingFlow({ date: inDays(-2), amount: Money.valid(7) }),
      ],
      NO_SUMMARY,
      today
    );

    expect(publisher.published()[0].map((entry) => entry.date)).toEqual([today, inDays(1), inDays(2), inDays(3)]);
    expect(publisher.published()[0][2]).toEqual({ date: inDays(2), summary: { pastDue: total(2, 10), dueToday: total(1, 5) } });
  });

  it('should keep only the earliest 64 entries', async () => {
    const flows = Array.from({ length: 70 }, (_, index) => aUpcomingFlow({ date: inDays(2 * (70 - index)) }));

    await useCase.execute(flows, NO_SUMMARY, today);

    const entries = publisher.published()[0];
    expect(entries).toHaveLength(64);
    expect(entries[0].date).toEqual(today);
    expect(entries[63].date).toEqual(inDays(64));
  });

  it('should fail when the widget cannot be published', async () => {
    publisher.isFailing(['error']);

    const result = await useCase.execute([aUpcomingFlow({ date: today })], NO_SUMMARY, today);

    expect(result).toBeFailureWithErrors(['error']);
  });

  it('should clear the widget', async () => {
    const result = await useCase.clear();

    expect(result).toBeSuccessWithValue(undefined);
    expect(publisher.clears()).toBe(1);
  });

  it('should total gains apart from expenses', async () => {
    await useCase.execute(
      [
        aUpcomingFlow({ date: inDays(-1), amount: Money.valid(30), categoryType: CategoryTypes.gain }),
        aUpcomingFlow({ date: inDays(-1), amount: Money.valid(12), categoryType: CategoryTypes.expense }),
        aUpcomingFlow({ date: today, amount: Money.valid(100), categoryType: CategoryTypes.gain }),
      ],
      NO_SUMMARY,
      today
    );

    expect(publisher.published()[0][0]).toEqual({ date: today, summary: { pastDue: total(2, 12, 30), dueToday: total(1, 0, 100) } });
  });

  it('should publish the spending and the projected net flow for the period', async () => {
    const summary = { ...NO_SUMMARY, currentSpending: 1847.63, netFlow: { amount: 400, isOver: true } };

    await useCase.execute([aUpcomingFlow({ date: today, amount: Money.valid(82.08), categoryType: CategoryTypes.expense })], summary, today);

    expect(publisher.periods()).toEqual([{ spending: 1847.63, projected: 317.92 }]);
  });
});
