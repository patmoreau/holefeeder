import { type AsyncResult, today } from '@holefeeder/shared/core';
import { waitFor } from '@testing-library/react-native';
import { aSettings } from '@/shared/core/__tests__/settings-for-test';
import { aCategoryIncome } from '@/statistics/__tests__/category-income-for-test';
import { InsightsRepositoryInMemory } from '@/statistics/__tests__/insights-repository-in-memory';
import { CategoryIncome } from '@/statistics/core/category-income';
import { WatchCategoryIncomeUseCase } from './watch-category-income-use-case';

describe('WatchCategoryIncomeUseCase', () => {
  const effectiveDate = today();
  const settings = aSettings();
  let repository: InsightsRepositoryInMemory;
  let useCase: ReturnType<typeof WatchCategoryIncomeUseCase>;

  beforeEach(() => {
    repository = InsightsRepositoryInMemory();
    useCase = WatchCategoryIncomeUseCase(repository, effectiveDate, settings);
  });

  it('returns category income when repository succeeds', async () => {
    const items = [aCategoryIncome(), aCategoryIncome()];
    repository.addCategoryIncome(...items);

    let result: AsyncResult<CategoryIncome[]> | undefined;
    const unsubscribe = useCase.watch((data) => {
      result = data;
    });

    await waitFor(() => expect(result).toBeDefined());

    expect(result).toBeSuccessWithValue(items);

    unsubscribe();
  });

  it('returns failure when repository fails', async () => {
    repository.isFailing(['error']);

    let result: AsyncResult<CategoryIncome[]> | undefined;
    const unsubscribe = useCase.watch((data) => {
      result = data;
    });

    await waitFor(() => expect(result).toBeDefined());

    expect(result).toBeFailureWithErrors(['error']);

    unsubscribe();
  });

  it('returns loading when repository is loading', async () => {
    repository.isLoading();

    let result: AsyncResult<CategoryIncome[]> | undefined;
    const unsubscribe = useCase.watch((data) => {
      result = data;
    });

    await waitFor(() => expect(result).toBeDefined());

    expect(result).toBeLoading();

    unsubscribe();
  });
});
