import { DateIntervalTypes, DateOnly, Result } from '@holefeeder/shared/core';
import { act, renderHook, waitFor } from '@testing-library/react-native';
import React from 'react';
import { aSettings } from '@/shared/core/__tests__/settings-for-test';
import { RepositoryContextForTest } from '@/shared/repositories/__tests__/RepositoryContextForTest';
import { SummaryRepositoryInMemory } from '@/summary/__tests__/summary-repository-in-memory';
import { usePeriodSeries } from '@/summary/presentation/core/use-period-series';

const mockUseSettings = jest.fn();
jest.mock('@/shared/presentation/core/use-settings', () => ({
  useSettings: () => mockUseSettings(),
}));

describe('usePeriodSeries', () => {
  let summaryRepository: SummaryRepositoryInMemory;

  const createHook = async () =>
    renderHook(() => usePeriodSeries(6), {
      wrapper: ({ children }: { children: React.ReactNode }) => (
        <RepositoryContextForTest repositories={{ summaryRepository }}>{children}</RepositoryContextForTest>
      ),
    });

  beforeEach(() => {
    summaryRepository = SummaryRepositoryInMemory();
    mockUseSettings.mockReturnValue(Result.success(aSettings({ intervalType: DateIntervalTypes.monthly, frequency: 1 })));
  });

  it('emits the requested number of periods when the repository succeeds', async () => {
    const { result } = await createHook();

    await waitFor(() => expect(result.current).not.toBeLoading());

    expect(result.current.isSuccess && result.current.value).toHaveLength(6);
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

  it('rebuilds the periods when the budget settings change', async () => {
    mockUseSettings.mockReturnValue(
      Result.success(aSettings({ effectiveDate: DateOnly.valid('2026-01-01'), intervalType: DateIntervalTypes.monthly, frequency: 1 }))
    );
    const { result, rerender } = await createHook();
    await waitFor(() => expect(result.current).not.toBeLoading());
    const monthly = result.current.isSuccess ? result.current.value : [];

    mockUseSettings.mockReturnValue(
      Result.success(aSettings({ effectiveDate: DateOnly.valid('2026-01-02'), intervalType: DateIntervalTypes.weekly, frequency: 2 }))
    );
    await act(async () => rerender({}));

    await waitFor(() => expect(result.current.isSuccess && result.current.value[5].from).not.toEqual(monthly[5].from));
  });
});
