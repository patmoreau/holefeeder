import { CategoryTypes, Money } from '@holefeeder/shared/core';
import { SpendingWidgetPublisherInMemory } from '@/dashboard/core/__tests__/spending-widget-publisher-in-memory';
import { PublishSpendingWidgetUseCase } from '@/dashboard/core/publish-spending-widget/publish-spending-widget-use-case';
import { aUpcomingFlow } from '@/flows/core/flows/__tests__/upcoming-flow-for-test';
import { NO_SUMMARY } from '@/summary/core/watch-summary/watch-summary-use-case';

describe('PublishSpendingWidgetUseCase', () => {
  let publisher: SpendingWidgetPublisherInMemory;
  let useCase: ReturnType<typeof PublishSpendingWidgetUseCase>;

  beforeEach(() => {
    publisher = SpendingWidgetPublisherInMemory();
    useCase = PublishSpendingWidgetUseCase(publisher);
  });

  it('should publish the spending and the projected net flow for the period', async () => {
    const summary = { ...NO_SUMMARY, currentSpending: 1847.63, netFlow: { amount: 400, isOver: true } };

    const result = await useCase.execute(summary, [aUpcomingFlow({ amount: Money.valid(82.08), categoryType: CategoryTypes.expense })]);

    expect(result).toBeSuccessWithValue(undefined);
    expect(publisher.published()).toEqual([{ spending: 1847.63, projected: 317.92 }]);
  });

  it('should publish how far spending is from the average', async () => {
    const summary = {
      ...NO_SUMMARY,
      currentSpending: 1847.63,
      averageSpending: 2060.03,
      variation: { amount: 212.4, percentage: 10.31, isOver: false },
    };

    await useCase.execute(summary, []);

    expect(publisher.published()[0].trend).toEqual({ amount: 212.4, isOver: false });
  });

  it('should leave the trend out when there is no average spending yet', async () => {
    const summary = { ...NO_SUMMARY, currentSpending: 50, variation: { amount: 50, percentage: 0, isOver: true } };

    await useCase.execute(summary, []);

    expect(publisher.published()[0]).toEqual({ spending: 50, projected: 0 });
  });

  it('should fail when the widget cannot be published', async () => {
    publisher.isFailing(['error']);

    const result = await useCase.execute(NO_SUMMARY, []);

    expect(result).toBeFailureWithErrors(['error']);
  });

  it('should clear the widget', async () => {
    const result = await useCase.clear();

    expect(result).toBeSuccessWithValue(undefined);
    expect(publisher.clears()).toBe(1);
  });
});
