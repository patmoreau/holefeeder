import { Result } from '@holefeeder/shared/core';
import { renderHook, waitFor } from '@testing-library/react-native';
import React from 'react';
import { aSettings } from '@/settings/core/__tests__/settings-for-test';
import { RepositoryContextForTest } from '@/shared/repositories/__tests__/RepositoryContextForTest';
import { InsightsRepositoryInMemory } from '@/statistics/__tests__/insights-repository-in-memory';
import { aTagSpending } from '@/statistics/__tests__/tag-spending-for-test';
import { useTagSpending } from '@/statistics/presentation/core/use-tag-spending';

const mockUseSettings = jest.fn();
jest.mock('@/shared/presentation/core/use-settings', () => ({
  useSettings: () => mockUseSettings(),
}));

describe('useTagSpending', () => {
  let insightsRepository: InsightsRepositoryInMemory;

  const createHook = async () =>
    renderHook(() => useTagSpending(), {
      wrapper: ({ children }: { children: React.ReactNode }) => (
        <RepositoryContextForTest repositories={{ insightsRepository }}>{children}</RepositoryContextForTest>
      ),
    });

  beforeEach(() => {
    insightsRepository = InsightsRepositoryInMemory();
    mockUseSettings.mockReturnValue(Result.success(aSettings()));
  });

  it('emits the tag spending when the repository succeeds', async () => {
    const items = [aTagSpending(), aTagSpending()];
    insightsRepository.addTagSpending(...items);

    const { result } = await createHook();

    await waitFor(() => expect(result.current).not.toBeLoading());

    expect(result.current).toBeSuccessWithValue(items);
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

    expect(result.current).toBeFailureWithErrors(['boom']);
  });
});
