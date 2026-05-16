import { waitFor } from '@testing-library/react-native';
import { startOfMonth } from 'date-fns';
import { DashboardRepositoryInMemory } from '@/dashboard/__tests__/dashboard-repository-in-memory';
import { aSummaryData } from '@/dashboard/__tests__/summary-data-for-test';
import { DashboardComputedSummary, WatchSummaryUseCase } from '@/dashboard/core/watch-summary/watch-summary-use-case';
import { CategoryTypes } from '@/flows/core/categories/category-type';
import { aSettings } from '@/settings/core/__tests__/settings-for-test';
import { DateIntervalTypes } from '@/shared/core/date-interval-type';
import { Money } from '@/shared/core/money';
import { type AsyncResult } from '@/shared/core/result';
import { today, withDate } from '@/shared/core/with-date';

describe('WatchCategoriesUseCase', () => {
  const asOfDate = withDate(startOfMonth(today())).toDateOnly();
  const settings = aSettings({
    effectiveDate: asOfDate,
    intervalType: DateIntervalTypes.monthly,
    frequency: 1,
  });
  let repository: DashboardRepositoryInMemory;
  let useCase: ReturnType<typeof WatchSummaryUseCase>;

  beforeEach(() => {
    repository = DashboardRepositoryInMemory();
    useCase = WatchSummaryUseCase(settings, repository);
  });

  it('returns summary when repository succeeds', async () => {
    const expenses = [
      aSummaryData({ date: asOfDate, type: CategoryTypes.expense, total: Money.valid(100) }),
      aSummaryData({ date: asOfDate, type: CategoryTypes.expense, total: Money.valid(200) }),
    ];
    const gains = [
      aSummaryData({ date: asOfDate, type: CategoryTypes.gain, total: Money.valid(1000) }),
      aSummaryData({ date: asOfDate, type: CategoryTypes.gain, total: Money.valid(2000) }),
    ];
    repository.add(...expenses, ...gains);

    let result: AsyncResult<DashboardComputedSummary> | undefined;
    const unsubscribe = useCase.watch((data) => {
      result = data;
    });

    await waitFor(() => expect(result).toBeDefined());

    expect(result).toBeSuccessWithValue({
      averageSpending: Money.valid(300),
      currentSpending: Money.valid(300),
      variation: {
        amount: 0,
        percentage: 0,
        isOver: false,
      },
      netFlow: {
        amount: Money.valid(2700),
        isOver: true,
      },
      totalIncome: Money.valid(3000),
    });

    unsubscribe();
  });

  it('returns failure when repository fails', async () => {
    repository.isFailing(['error']);

    let result: AsyncResult<DashboardComputedSummary> | undefined;
    const unsubscribe = useCase.watch((data) => {
      result = data;
    });

    await waitFor(() => expect(result).toBeDefined());

    expect(result).toBeFailureWithErrors(['error']);

    unsubscribe();
  });

  it('returns loading when repository is loading', async () => {
    repository.isLoading();

    let result: AsyncResult<DashboardComputedSummary> | undefined;
    const unsubscribe = useCase.watch((data) => {
      result = data;
    });

    await waitFor(() => expect(result).toBeDefined());

    expect(result).toBeLoading();

    unsubscribe();
  });
});
