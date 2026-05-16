import { act, renderHook, waitFor } from '@testing-library/react-native';
import React from 'react';
import { aCategory, toCategory } from '@/flows/core/categories/__tests__/category-for-test';
import { useCategories } from '@/flows/presentation/shared/core/use-categories';
import { DatabaseForTest, setupDatabaseForTest } from '@/shared/persistence/__tests__/database-for-test';
import { PowerSyncProviderForTest } from '@/shared/persistence/__tests__/PowerSyncProviderForTest';

describe('useCategories', () => {
  let db: DatabaseForTest;
  const category = aCategory();

  const createHook = async () =>
    await waitFor(() =>
      renderHook(() => useCategories(), {
        wrapper: ({ children }: { children: React.ReactNode }) => <PowerSyncProviderForTest database={db}>{children}</PowerSyncProviderForTest>,
      })
    );

  beforeEach(async () => {
    db = await setupDatabaseForTest();

    await category.store(db);
  });

  afterEach(async () => {
    await act(async () => {
      await db.cleanupTestDb();
    });
  });

  it('should fetch categories from PowerSync database', async () => {
    const { result } = await createHook();
    // Initially loading
    expect(result.current).toBeLoading();

    // Wait for data to load
    await waitFor(() => expect(result.current).not.toBeLoading());

    expect(result.current).toBeSuccessWithValue([toCategory(category)]);
  });
});
