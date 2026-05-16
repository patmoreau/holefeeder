import { act, renderHook, waitFor } from '@testing-library/react-native';
import React from 'react';
import { aSettings } from '@/settings/core/__tests__/settings-for-test';
import { aStoreItem } from '@/shared/__tests__/store-item-for-test';
import { DatabaseForTest, setupDatabaseForTest } from '@/shared/persistence/__tests__/database-for-test';
import { PowerSyncProviderForTest } from '@/shared/persistence/__tests__/PowerSyncProviderForTest';
import { useSettings } from '@/shared/presentation/core/use-settings';

describe('useStoreItems', () => {
  let db: DatabaseForTest;
  const settings = aSettings();
  const storeItem = aStoreItem({ code: 'settings', data: JSON.stringify(settings) });

  const createHook = async () =>
    await waitFor(() =>
      renderHook(() => useSettings(), {
        wrapper: ({ children }: { children: React.ReactNode }) => <PowerSyncProviderForTest database={db}>{children}</PowerSyncProviderForTest>,
      })
    );

  beforeEach(async () => {
    db = await setupDatabaseForTest();

    await storeItem.store(db);
  });

  afterEach(async () => {
    await act(async () => {
      await db.cleanupTestDb();
    });
  });

  it('should fetch settings from PowerSync database', async () => {
    const { result } = await createHook();
    // Initially loading
    expect(result.current).toBeLoading();

    // Wait for data to load
    await waitFor(() => expect(result.current).not.toBeLoading());

    expect(result.current).toBeSuccessWithValue(settings);
  });
});
