import { act, renderHook, RenderHookResult, waitFor } from '@testing-library/react-native';
import React from 'react';
import { AppStorageInMemory } from '@/shared/__tests__/app-storage-in-memory';
import { CombinedInsightProvider } from '@/statistics/presentation/CombinedInsightProvider';
import { CombinedInsightState } from '@/statistics/presentation/core/combined-insight-state';
import { useCombinedInsight } from '@/statistics/presentation/core/use-combined-insight';

const COMBINED_KEY = 'app_settings_combined_insight';

describe('useCombinedInsight', () => {
  const createHook = async (storage: AppStorageInMemory): Promise<RenderHookResult<CombinedInsightState, unknown>> =>
    await waitFor(() =>
      renderHook(() => useCombinedInsight(), {
        wrapper: ({ children }: { children: React.ReactNode }) => (
          <CombinedInsightProvider storage={storage}>{children}</CombinedInsightProvider>
        ),
      })
    );

  it('defaults to false when storage is empty', async () => {
    const storage = AppStorageInMemory();

    const hook = await createHook(storage);

    expect(hook.result.current.combined).toBe(false);
  });

  it('restores true when storage holds true', async () => {
    const storage = AppStorageInMemory();
    storage.setString(COMBINED_KEY, 'true');

    const hook = await createHook(storage);

    expect(hook.result.current.combined).toBe(true);
  });

  it('persists to storage when setCombined is called', async () => {
    const storage = AppStorageInMemory();
    const hook = await createHook(storage);

    await act(async () => {
      hook.result.current.setCombined(true);
    });

    expect(hook.result.current.combined).toBe(true);
    expect(storage.getString(COMBINED_KEY)).toBe('true');
  });

  it('throws when used outside a provider', async () => {
    await expect(renderHook(() => useCombinedInsight())).rejects.toThrow('useCombinedInsight must be used within a CombinedInsightProvider');
  });
});
