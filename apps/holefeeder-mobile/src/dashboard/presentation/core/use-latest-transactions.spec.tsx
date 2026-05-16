import { act, renderHook, waitFor } from '@testing-library/react-native';
import React from 'react';
import { useLatestTransactions } from '@/dashboard/presentation/core/use-latest-transactions';
import { aCategory } from '@/flows/core/categories/__tests__/category-for-test';
import { CategoryTypes } from '@/flows/core/categories/category-type';
import { aTransaction } from '@/flows/core/flows/__tests__/transaction-for-test';
import { DateOnly } from '@/shared/core/date-only';
import { Money } from '@/shared/core/money';
import { DatabaseForTest, setupDatabaseForTest } from '@/shared/persistence/__tests__/database-for-test';
import { PowerSyncProviderForTest } from '@/shared/persistence/__tests__/PowerSyncProviderForTest';

describe('useLatestTransactions', () => {
  let db: DatabaseForTest;

  const createHook = async (limit?: number) =>
    await waitFor(() =>
      renderHook(() => useLatestTransactions(limit), {
        wrapper: ({ children }: { children: React.ReactNode }) => <PowerSyncProviderForTest database={db}>{children}</PowerSyncProviderForTest>,
      })
    );

  beforeEach(async () => {
    db = await setupDatabaseForTest();
  });

  afterEach(async () => {
    await act(async () => {
      await db.cleanupTestDb();
    });
  });

  it('should start in loading state and resolve with transactions', async () => {
    const category = aCategory({ type: CategoryTypes.expense });
    await category.store(db);
    await aTransaction({ categoryId: category.id, amount: Money.valid(50.0), date: DateOnly.valid('2026-03-01') }).store(db);

    const { result } = await createHook();

    expect(result.current.data).toBeLoading();

    await waitFor(() => expect(result.current).not.toBeLoading());

    expect(result.current.data).toBeSuccessWithValue(expect.anything());
  });

  it('should return an empty list when no transactions exist', async () => {
    const { result } = await createHook();

    await waitFor(() => expect(result.current.data).not.toBeLoading());

    expect(result.current.data).toBeSuccessWithValue([]);
  });

  it('should respect the limit parameter', async () => {
    const category = aCategory({ type: CategoryTypes.expense });
    await category.store(db);
    await aTransaction({ categoryId: category.id, date: DateOnly.valid('2026-01-01') }).store(db);
    await aTransaction({ categoryId: category.id, date: DateOnly.valid('2026-02-01') }).store(db);
    await aTransaction({ categoryId: category.id, date: DateOnly.valid('2026-03-01') }).store(db);

    const { result } = await createHook(2);

    await waitFor(() => expect(result.current.data).not.toBeLoading());

    expect(result.current.data.isSuccess).toBe(true);
    if (result.current.data.isSuccess) {
      expect(result.current.data.value).toHaveLength(2);
    }
  });
});
