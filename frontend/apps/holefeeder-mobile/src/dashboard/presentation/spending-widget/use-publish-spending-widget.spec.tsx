import { CategoryTypes, DateIntervalTypes, Logger, loggerFactoryForNoop, Money, today, User } from '@holefeeder/shared/core';
import { aUser } from '@holefeeder/shared/testkit';
import { act, renderHook, waitFor } from '@testing-library/react-native';
import React from 'react';
import { AppState, AppStateStatus } from 'react-native';
import { anAccount } from '@/accounts/core/__tests__/account-for-test';
import { SpendingWidgetPublisherInMemory } from '@/dashboard/core/__tests__/spending-widget-publisher-in-memory';
import { usePublishSpendingWidget } from '@/dashboard/presentation/spending-widget/use-publish-spending-widget';
import { aCategory } from '@/flows/core/categories/__tests__/category-for-test';
import { aCashflow } from '@/flows/core/flows/__tests__/cashflow-for-test';
import { aTransaction } from '@/flows/core/flows/__tests__/transaction-for-test';
import { aStoreItem } from '@/shared/__tests__/store-item-for-test';
import { AuthenticationContextForTest } from '@/shared/auth/__tests__/AuthenticationContextForTest';
import { aSettings } from '@/shared/core/__tests__/settings-for-test';
import { SETTINGS_CODE } from '@/shared/core/settings';
import { DatabaseForTest, setupDatabaseForTest } from '@/shared/persistence/__tests__/database-for-test';
import { PowerSyncProviderForTest } from '@/shared/persistence/__tests__/PowerSyncProviderForTest';

describe('usePublishSpendingWidget', () => {
  let db: DatabaseForTest;
  let publisher: SpendingWidgetPublisherInMemory;
  let signedInUser: User | undefined;
  const account = anAccount();
  const category = aCategory({ type: CategoryTypes.expense });

  const wrapper = ({ children }: { children: React.ReactNode }) => (
    <AuthenticationContextForTest overrides={{ user: signedInUser }}>
      <PowerSyncProviderForTest database={db}>{children}</PowerSyncProviderForTest>
    </AuthenticationContextForTest>
  );

  const createHook = async () => await renderHook(() => usePublishSpendingWidget(publisher), { wrapper: wrapper });

  const anExpense = (amount: number) =>
    aTransaction({ accountId: account.id, categoryId: category.id, date: today(), amount: Money.valid(amount) });

  beforeEach(async () => {
    db = await setupDatabaseForTest();
    publisher = SpendingWidgetPublisherInMemory();
    signedInUser = aUser();
    const settings = aSettings({ effectiveDate: today(), intervalType: DateIntervalTypes.monthly, frequency: 1 });
    await aStoreItem({ code: SETTINGS_CODE, data: JSON.stringify(settings) }).store(db);
    await account.store(db);
    await category.store(db);
  });

  afterEach(async () => {
    await act(async () => {
      await db.cleanupTestDb();
    });
  });

  it('should publish the spending and the projected net flow for the period', async () => {
    await anExpense(40).store(db);
    await aCashflow({
      accountId: account.id,
      categoryId: category.id,
      effectiveDate: today(),
      amount: Money.valid(10),
      intervalType: DateIntervalTypes.oneTime,
      frequency: 1,
    }).store(db);

    await createHook();

    await waitFor(() => expect(publisher.published().at(-1)).toEqual({ spending: 40, trend: { amount: 0, isOver: false }, projected: -50 }));
  });

  it('should publish again when the app becomes active', async () => {
    const handlers: ((state: AppStateStatus) => void)[] = [];
    jest.spyOn(AppState, 'addEventListener').mockImplementation((_, handler) => {
      handlers.push(handler);
      return { remove: () => {} };
    });
    await createHook();
    await waitFor(() => expect(publisher.published().length).toBeGreaterThan(0));
    const count = publisher.published().length;

    await act(async () => handlers.forEach((handler) => handler('active')));

    await waitFor(() => expect(publisher.published().length).toBeGreaterThan(count));
  });

  it('should log and keep running when the widget cannot be published', async () => {
    const errors: unknown[][] = [];
    Logger.setLoggerFactory(() => ({ debug: () => {}, info: () => {}, warn: () => {}, error: (...args: unknown[]) => errors.push(args) }));
    publisher.isFailing(['publish-spending-widget-failed']);

    await createHook();

    await waitFor(() => expect(errors).toContainEqual([expect.any(String), ['publish-spending-widget-failed']]));
    Logger.setLoggerFactory(loggerFactoryForNoop);
  });

  it('should clear the widget when signed out', async () => {
    signedInUser = undefined;

    await createHook();

    await waitFor(() => expect(publisher.clears()).toBeGreaterThan(0));
    expect(publisher.published()).toEqual([]);
  });

  it('should clear the widget when the user signs out', async () => {
    const hook = await createHook();
    await waitFor(() => expect(publisher.published().length).toBeGreaterThan(0));
    const clears = publisher.clears();

    signedInUser = undefined;
    await act(async () => hook.rerender({}));

    await waitFor(() => expect(publisher.clears()).toBeGreaterThan(clears));
  });

  it('should clear the widget when it stops running', async () => {
    const hook = await createHook();
    await waitFor(() => expect(publisher.published().length).toBeGreaterThan(0));
    const clears = publisher.clears();

    await act(async () => hook.unmount());

    await waitFor(() => expect(publisher.clears()).toBeGreaterThan(clears));
  });
});
