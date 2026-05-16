import { DateIntervalTypes, DateOnly, Money, Variation } from '@holefeeder/core';
import { act, renderHook, waitFor } from '@testing-library/react-native';
import React from 'react';
import { anAccount } from '@/accounts/core/__tests__/account-for-test';
import { AccountTypes } from '@/accounts/core/account-type';
import { useAccountDetails } from '@/dashboard/presentation/core/use-account-details';
import { aCategory } from '@/flows/core/categories/__tests__/category-for-test';
import { CategoryTypes } from '@/flows/core/categories/category-type';
import { aCashflow } from '@/flows/core/flows/__tests__/cashflow-for-test';
import { aTransaction } from '@/flows/core/flows/__tests__/transaction-for-test';
import { aSettings } from '@/settings/core/__tests__/settings-for-test';
import { SETTINGS_CODE } from '@/settings/core/settings';
import { aStoreItem } from '@/shared/__tests__/store-item-for-test';
import { DatabaseForTest, setupDatabaseForTest } from '@/shared/persistence/__tests__/database-for-test';
import { PowerSyncProviderForTest } from '@/shared/persistence/__tests__/PowerSyncProviderForTest';

describe('useAccountDetails', () => {
  let db: DatabaseForTest;
  const account = anAccount({ openBalance: Variation.valid(100), type: AccountTypes.creditCard });
  const category = aCategory({ type: CategoryTypes.expense });
  const cashflow = aCashflow({
    accountId: account.id,
    categoryId: category.id,
    amount: Money.valid(123.45),
    effectiveDate: DateOnly.valid('2025-01-01'),
    intervalType: DateIntervalTypes.monthly,
    frequency: 1,
  });
  const transaction = aTransaction({
    accountId: account.id,
    categoryId: category.id,
    amount: Money.valid(123.45),
    date: DateOnly.valid('2025-01-01'),
    cashflowId: cashflow.id,
    cashflowDate: cashflow.effectiveDate,
  });
  const settings = aSettings({
    effectiveDate: DateOnly.valid('2025-01-01'),
    intervalType: DateIntervalTypes.monthly,
    frequency: 1,
  });

  const createHook = async () =>
    await waitFor(() =>
      renderHook(() => useAccountDetails(), {
        wrapper: ({ children }: { children: React.ReactNode }) => <PowerSyncProviderForTest database={db}>{children}</PowerSyncProviderForTest>,
      })
    );

  beforeEach(async () => {
    db = await setupDatabaseForTest();

    await aStoreItem({ code: SETTINGS_CODE, data: JSON.stringify(settings) }).store(db);
    await account.store(db);
    await category.store(db);
    await cashflow.store(db);
    await transaction.store(db);
  });

  afterEach(async () => {
    await act(async () => {
      await db.cleanupTestDb();
    });
  });

  it('should fetch accounts from PowerSync database', async () => {
    const { result } = await createHook();

    expect(result.current).toBeLoading();

    await waitFor(() => expect(result.current).not.toBeLoading());

    expect(result.current).toBeSuccessWithValue(expect.anything());
  });
});
