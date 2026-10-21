import { type AsyncResult, today } from '@holefeeder/shared/core';
import { waitFor } from '@testing-library/react-native';
import { aSettings } from '@/shared/core/__tests__/settings-for-test';
import { InsightsRepositoryInMemory } from '@/statistics/__tests__/insights-repository-in-memory';
import { aTagIncome } from '@/statistics/__tests__/tag-income-for-test';
import { TagIncome } from '@/statistics/core/tag-income';
import { WatchTagIncomeUseCase } from './watch-tag-income-use-case';

describe('WatchTagIncomeUseCase', () => {
  const effectiveDate = today();
  const settings = aSettings();
  let repository: InsightsRepositoryInMemory;
  let useCase: ReturnType<typeof WatchTagIncomeUseCase>;

  beforeEach(() => {
    repository = InsightsRepositoryInMemory();
    useCase = WatchTagIncomeUseCase(repository, effectiveDate, settings);
  });

  it('returns tag income when repository succeeds', async () => {
    const items = [aTagIncome(), aTagIncome()];
    repository.addTagIncome(...items);

    let result: AsyncResult<TagIncome[]> | undefined;
    const unsubscribe = useCase.watch((data) => {
      result = data;
    });

    await waitFor(() => expect(result).toBeDefined());

    expect(result).toBeSuccessWithValue(items);

    unsubscribe();
  });

  it('returns failure when repository fails', async () => {
    repository.isFailing(['error']);

    let result: AsyncResult<TagIncome[]> | undefined;
    const unsubscribe = useCase.watch((data) => {
      result = data;
    });

    await waitFor(() => expect(result).toBeDefined());

    expect(result).toBeFailureWithErrors(['error']);

    unsubscribe();
  });

  it('returns loading when repository is loading', async () => {
    repository.isLoading();

    let result: AsyncResult<TagIncome[]> | undefined;
    const unsubscribe = useCase.watch((data) => {
      result = data;
    });

    await waitFor(() => expect(result).toBeDefined());

    expect(result).toBeLoading();

    unsubscribe();
  });
});
