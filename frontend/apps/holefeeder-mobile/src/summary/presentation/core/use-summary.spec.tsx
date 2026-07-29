import { Result } from '@holefeeder/shared/core';
import { renderHook, waitFor } from '@testing-library/react-native';
import React from 'react';
import { aSettings } from '@/shared/core/__tests__/settings-for-test';
import { RepositoryContextForTest } from '@/shared/repositories/__tests__/RepositoryContextForTest';
import { SummaryRepositoryInMemory } from '@/summary/__tests__/summary-repository-in-memory';
import { useSummary } from '@/summary/presentation/core/use-summary';

const mockUseSettings = jest.fn();
jest.mock('@/shared/presentation/core/use-settings', () => ({
  useSettings: () => mockUseSettings(),
}));

describe('useSummary', () => {
  let summaryRepository: SummaryRepositoryInMemory;

  const createHook = async () =>
    renderHook(() => useSummary(), {
      wrapper: ({ children }: { children: React.ReactNode }) => (
        <RepositoryContextForTest repositories={{ summaryRepository }}>{children}</RepositoryContextForTest>
      ),
    });

  beforeEach(() => {
    summaryRepository = SummaryRepositoryInMemory();
    mockUseSettings.mockReturnValue(Result.success(aSettings()));
  });

  it('emits a computed summary when the repository succeeds', async () => {
    const { result } = await createHook();

    await waitFor(() => expect(result.current).not.toBeLoading());

    expect(result.current.isSuccess).toBe(true);
  });

  it('is loading while the repository is loading', async () => {
    summaryRepository.isLoading();

    const { result } = await createHook();

    expect(result.current).toBeLoading();
  });

  it('propagates a repository failure', async () => {
    summaryRepository.isFailing(['boom']);

    const { result } = await createHook();

    await waitFor(() => expect(result.current).not.toBeLoading());

    expect(result.current).toBeFailureWithErrors(['boom']);
  });
});
