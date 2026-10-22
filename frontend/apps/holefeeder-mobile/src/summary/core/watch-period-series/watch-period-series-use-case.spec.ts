import { type AsyncResult, DateIntervalTypes, DateOnly, Money } from '@holefeeder/shared/core';
import { waitFor } from '@testing-library/react-native';
import { aSettings } from '@/shared/core/__tests__/settings-for-test';
import { CategoryTypes } from '@/shared/core/category-type';
import { aSummaryData } from '@/summary/__tests__/summary-data-for-test';
import { SummaryRepositoryInMemory } from '@/summary/__tests__/summary-repository-in-memory';
import { PeriodTotals } from '@/summary/core/period-series';
import { WatchPeriodSeriesUseCase } from './watch-period-series-use-case';

describe('WatchPeriodSeriesUseCase', () => {
  const settings = aSettings({ effectiveDate: DateOnly.valid('2026-01-01'), intervalType: DateIntervalTypes.monthly, frequency: 1 });
  const asOf = DateOnly.valid('2026-10-21');
  let repository: SummaryRepositoryInMemory;

  beforeEach(() => {
    repository = SummaryRepositoryInMemory();
  });

  const watch = async () => {
    let result: AsyncResult<PeriodTotals[]> | undefined;
    const unsubscribe = WatchPeriodSeriesUseCase(settings, repository, asOf, 2).watch((data) => {
      result = data;
    });
    await waitFor(() => expect(result).toBeDefined());
    unsubscribe();
    return result;
  };

  it('returns the last periods of income and expenses', async () => {
    repository.add(
      aSummaryData({ type: CategoryTypes.gain, date: DateOnly.valid('2026-09-01'), total: Money.valid(4200) }),
      aSummaryData({ type: CategoryTypes.expense, date: DateOnly.valid('2026-10-02'), total: Money.valid(3260) })
    );

    expect(await watch()).toBeSuccessWithValue([
      { from: DateOnly.valid('2026-09-01'), to: DateOnly.valid('2026-09-30'), gains: Money.valid(4200), expenses: Money.ZERO },
      { from: DateOnly.valid('2026-10-01'), to: DateOnly.valid('2026-10-31'), gains: Money.ZERO, expenses: Money.valid(3260) },
    ]);
  });

  it('returns failure when repository fails', async () => {
    repository.isFailing(['error']);

    expect(await watch()).toBeFailureWithErrors(['error']);
  });

  it('returns loading when repository is loading', async () => {
    repository.isLoading();

    expect(await watch()).toBeLoading();
  });
});
