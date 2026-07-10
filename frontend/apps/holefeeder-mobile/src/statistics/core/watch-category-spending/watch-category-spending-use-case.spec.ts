import { type AsyncResult, today } from '@holefeeder/shared/core';
import { waitFor } from '@testing-library/react-native';
import { aSettings } from '@/settings/core/__tests__/settings-for-test';
import { aCategorySpending } from '@/statistics/__tests__/category-spending-for-test';
import { InsightsRepositoryInMemory } from '@/statistics/__tests__/insights-repository-in-memory';
import { CategorySpending } from '@/statistics/core/category-spending';
import { WatchCategorySpendingUseCase } from './watch-category-spending-use-case';

describe('WatchCategorySpendingUseCase', () => {
  const effectiveDate = today();
  const settings = aSettings();
  let repository: InsightsRepositoryInMemory;
  let useCase: ReturnType<typeof WatchCategorySpendingUseCase>;

  beforeEach(() => {
    repository = InsightsRepositoryInMemory();
    useCase = WatchCategorySpendingUseCase(repository, effectiveDate, settings);
  });

  it('returns category spending when repository succeeds', async () => {
    const items = [aCategorySpending(), aCategorySpending()];
    repository.addCategorySpending(...items);

    let result: AsyncResult<CategorySpending[]> | undefined;
    const unsubscribe = useCase.watch((data) => {
      result = data;
    });

    await waitFor(() => expect(result).toBeDefined());

    expect(result).toBeSuccessWithValue(items);

    unsubscribe();
  });

  it('returns failure when repository fails', async () => {
    repository.isFailing(['error']);

    let result: AsyncResult<CategorySpending[]> | undefined;
    const unsubscribe = useCase.watch((data) => {
      result = data;
    });

    await waitFor(() => expect(result).toBeDefined());

    expect(result).toBeFailureWithErrors(['error']);

    unsubscribe();
  });

  it('returns loading when repository is loading', async () => {
    repository.isLoading();

    let result: AsyncResult<CategorySpending[]> | undefined;
    const unsubscribe = useCase.watch((data) => {
      result = data;
    });

    await waitFor(() => expect(result).toBeDefined());

    expect(result).toBeLoading();

    unsubscribe();
  });
});
