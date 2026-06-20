import { type AsyncResult } from '@holefeeder/shared/core';
import { waitFor } from '@testing-library/react-native';
import { aSettings } from '@/settings/core/__tests__/settings-for-test';
import { InsightsRepositoryInMemory } from '@/statistics/__tests__/insights-repository-in-memory';
import { aTagSpending } from '@/statistics/__tests__/tag-spending-for-test';
import { TagSpending } from '@/statistics/core/tag-spending';
import { WatchTagSpendingUseCase } from './watch-tag-spending-use-case';

describe('WatchTagSpendingUseCase', () => {
  const settings = aSettings();
  let repository: InsightsRepositoryInMemory;
  let useCase: ReturnType<typeof WatchTagSpendingUseCase>;

  beforeEach(() => {
    repository = InsightsRepositoryInMemory();
    useCase = WatchTagSpendingUseCase(repository, settings);
  });

  it('returns tag spending when repository succeeds', async () => {
    const items = [aTagSpending(), aTagSpending()];
    repository.addTagSpending(...items);

    let result: AsyncResult<TagSpending[]> | undefined;
    const unsubscribe = useCase.watch((data) => {
      result = data;
    });

    await waitFor(() => expect(result).toBeDefined());

    expect(result).toBeSuccessWithValue(items);

    unsubscribe();
  });

  it('returns failure when repository fails', async () => {
    repository.isFailing(['error']);

    let result: AsyncResult<TagSpending[]> | undefined;
    const unsubscribe = useCase.watch((data) => {
      result = data;
    });

    await waitFor(() => expect(result).toBeDefined());

    expect(result).toBeFailureWithErrors(['error']);

    unsubscribe();
  });

  it('returns loading when repository is loading', async () => {
    repository.isLoading();

    let result: AsyncResult<TagSpending[]> | undefined;
    const unsubscribe = useCase.watch((data) => {
      result = data;
    });

    await waitFor(() => expect(result).toBeDefined());

    expect(result).toBeLoading();

    unsubscribe();
  });
});
