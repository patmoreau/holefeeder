import { act, renderHook, RenderHookResult, waitFor } from '@testing-library/react-native';
import React from 'react';
import { AppStorageInMemory } from '@/shared/__tests__/app-storage-in-memory';
import { InsightsRangeState } from '@/statistics/presentation/core/insights-range-state';
import { useInsightsRange } from '@/statistics/presentation/core/use-insights-range';
import { InsightsRangeProvider } from '@/statistics/presentation/InsightsRangeProvider';

const RANGE_KEY = 'app_settings_insights_range';

describe('useInsightsRange', () => {
  const createHook = async (storage: AppStorageInMemory): Promise<RenderHookResult<InsightsRangeState, unknown>> =>
    await waitFor(() =>
      renderHook(() => useInsightsRange(), {
        wrapper: ({ children }: { children: React.ReactNode }) => <InsightsRangeProvider storage={storage}>{children}</InsightsRangeProvider>,
      })
    );

  it('defaults to six periods when storage is empty', async () => {
    const hook = await createHook(AppStorageInMemory());

    expect(hook.result.current.periodCount).toBe(6);
    expect(hook.result.current.options).toEqual([3, 6, 12]);
  });

  it('restores the stored range', async () => {
    const storage = AppStorageInMemory();
    storage.setString(RANGE_KEY, '12');

    const hook = await createHook(storage);

    expect(hook.result.current.periodCount).toBe(12);
  });

  it('falls back to six periods for an unknown stored value', async () => {
    const storage = AppStorageInMemory();
    storage.setString(RANGE_KEY, '7');

    const hook = await createHook(storage);

    expect(hook.result.current.periodCount).toBe(6);
  });

  it('persists the range when it changes', async () => {
    const storage = AppStorageInMemory();
    const hook = await createHook(storage);

    await act(async () => {
      hook.result.current.setPeriodCount(3);
    });

    expect(hook.result.current.periodCount).toBe(3);
    expect(storage.getString(RANGE_KEY)).toBe('3');
  });

  it('throws when used outside a provider', async () => {
    await expect(renderHook(() => useInsightsRange())).rejects.toThrow('useInsightsRange must be used within an InsightsRangeProvider');
  });
});
