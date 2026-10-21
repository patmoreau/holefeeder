import { Result } from '@holefeeder/shared/core';
import { renderHook, waitFor } from '@testing-library/react-native';
import React from 'react';
import { aSettings } from '@/shared/core/__tests__/settings-for-test';
import { RepositoryContextForTest } from '@/shared/repositories/__tests__/RepositoryContextForTest';
import { InsightsRepositoryInMemory } from '@/statistics/__tests__/insights-repository-in-memory';
import { aTagIncome } from '@/statistics/__tests__/tag-income-for-test';
import { useTagIncome } from '@/statistics/presentation/core/use-tag-income';

const mockUseSettings = jest.fn();
jest.mock('@/shared/presentation/core/use-settings', () => ({
  useSettings: () => mockUseSettings(),
}));

describe('useTagIncome', () => {
  let insightsRepository: InsightsRepositoryInMemory;

  const createHook = async () =>
    renderHook(() => useTagIncome(), {
      wrapper: ({ children }: { children: React.ReactNode }) => (
        <RepositoryContextForTest repositories={{ insightsRepository }}>{children}</RepositoryContextForTest>
      ),
    });

  beforeEach(() => {
    insightsRepository = InsightsRepositoryInMemory();
    mockUseSettings.mockReturnValue(Result.success(aSettings()));
  });

  it('emits the tag income when the repository succeeds', async () => {
    const items = [aTagIncome(), aTagIncome()];
    insightsRepository.addTagIncome(...items);

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
