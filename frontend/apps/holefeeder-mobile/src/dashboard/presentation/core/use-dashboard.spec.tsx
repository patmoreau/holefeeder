import { Result } from '@holefeeder/shared/core';
import { renderHook, waitFor } from '@testing-library/react-native';
import React from 'react';
import { DashboardRepositoryInMemory } from '@/dashboard/__tests__/dashboard-repository-in-memory';
import { useDashboard } from '@/dashboard/presentation/core/use-dashboard';
import { aSettings } from '@/shared/core/__tests__/settings-for-test';
import { RepositoryContextForTest } from '@/shared/repositories/__tests__/RepositoryContextForTest';

const mockUseSettings = jest.fn();
jest.mock('@/shared/presentation/core/use-settings', () => ({
  useSettings: () => mockUseSettings(),
}));

describe('useDashboard', () => {
  let dashboardRepository: DashboardRepositoryInMemory;

  const createHook = async () =>
    renderHook(() => useDashboard(), {
      wrapper: ({ children }: { children: React.ReactNode }) => (
        <RepositoryContextForTest repositories={{ dashboardRepository }}>{children}</RepositoryContextForTest>
      ),
    });

  beforeEach(() => {
    dashboardRepository = DashboardRepositoryInMemory();
    mockUseSettings.mockReturnValue(Result.success(aSettings()));
  });

  it('emits a computed summary when the repository succeeds', async () => {
    const { result } = await createHook();

    await waitFor(() => expect(result.current).not.toBeLoading());

    expect(result.current.isSuccess).toBe(true);
  });

  it('is loading while the repository is loading', async () => {
    dashboardRepository.isLoading();

    const { result } = await createHook();

    expect(result.current).toBeLoading();
  });

  it('propagates a repository failure', async () => {
    dashboardRepository.isFailing(['boom']);

    const { result } = await createHook();

    await waitFor(() => expect(result.current).not.toBeLoading());

    expect(result.current).toBeFailureWithErrors(['boom']);
  });
});
