import { Money, Result } from '@holefeeder/shared/core';
import { renderHook, waitFor } from '@testing-library/react-native';
import React from 'react';
import { aSettings } from '@/shared/core/__tests__/settings-for-test';
import { RepositoryContextForTest } from '@/shared/repositories/__tests__/RepositoryContextForTest';
import { aCategorySpending } from '@/statistics/__tests__/category-spending-for-test';
import { aCategoryTagSpending } from '@/statistics/__tests__/category-tag-spending-for-test';
import { InsightsRepositoryInMemory } from '@/statistics/__tests__/insights-repository-in-memory';
import { useCombinedSpending } from '@/statistics/presentation/core/use-combined-spending';

const mockUseSettings = jest.fn();
jest.mock('@/shared/presentation/core/use-settings', () => ({
  useSettings: () => mockUseSettings(),
}));

describe('useCombinedSpending', () => {
  let insightsRepository: InsightsRepositoryInMemory;

  const createHook = async () =>
    renderHook(() => useCombinedSpending(), {
      wrapper: ({ children }: { children: React.ReactNode }) => (
        <RepositoryContextForTest repositories={{ insightsRepository }}>{children}</RepositoryContextForTest>
      ),
    });

  beforeEach(() => {
    insightsRepository = InsightsRepositoryInMemory();
    mockUseSettings.mockReturnValue(Result.success(aSettings()));
  });

  it('nests each category with its tags sorted by spend descending', async () => {
    const food = aCategorySpending();
    const home = aCategorySpending();
    insightsRepository.addCategorySpending(food, home);
    insightsRepository.addCategoryTagSpending(
      aCategoryTagSpending({ categoryId: food.categoryId, tag: 'groceries', spentAmount: Money.valid(100) }),
      aCategoryTagSpending({ categoryId: food.categoryId, tag: 'travel', spentAmount: Money.valid(250) }),
      aCategoryTagSpending({ categoryId: home.categoryId, tag: 'utilities', spentAmount: Money.valid(500) })
    );

    const { result } = await createHook();

    await waitFor(() => expect(result.current).not.toBeLoading());

    expect(result.current).toBeSuccessWithValue([
      {
        category: food,
        tags: [
          expect.objectContaining({ tag: 'travel', spentAmount: Money.valid(250) }),
          expect.objectContaining({ tag: 'groceries', spentAmount: Money.valid(100) }),
        ],
      },
      {
        category: home,
        tags: [expect.objectContaining({ tag: 'utilities', spentAmount: Money.valid(500) })],
      },
    ]);
  });

  it('returns an empty tag list for a category with no tags', async () => {
    const category = aCategorySpending();
    insightsRepository.addCategorySpending(category);

    const { result } = await createHook();

    await waitFor(() => expect(result.current).not.toBeLoading());

    expect(result.current).toBeSuccessWithValue([{ category, tags: [] }]);
  });

  it('is loading while the repository is loading', async () => {
    insightsRepository.isLoading();

    const { result } = await createHook();

    expect(result.current).toBeLoading();
  });

  it('propagates a repository failure', async () => {
    insightsRepository.isFailing(['boom']);

    const { result } = await createHook();

    await waitFor(() => expect(result.current).not.toBeLoading());

    expect(result.current).toBeFailureWithErrors(['boom', 'boom']);
  });
});
