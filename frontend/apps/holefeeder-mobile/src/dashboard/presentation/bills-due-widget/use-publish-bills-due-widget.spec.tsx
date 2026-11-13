import {
  CategoryTypes,
  DateIntervalTypes,
  DateOnly,
  Logger,
  loggerFactoryForNoop,
  Money,
  today,
  User,
  withDate,
} from '@holefeeder/shared/core';
import { aUser } from '@holefeeder/shared/testkit';
import { act, renderHook, waitFor } from '@testing-library/react-native';
import React from 'react';
import { AppState, AppStateStatus } from 'react-native';
import { anAccount } from '@/accounts/core/__tests__/account-for-test';
import { BillsDueWidgetPublisherInMemory } from '@/dashboard/core/__tests__/bills-due-widget-publisher-in-memory';
import { usePublishBillsDueWidget } from '@/dashboard/presentation/bills-due-widget/use-publish-bills-due-widget';
import { aCategory } from '@/flows/core/categories/__tests__/category-for-test';
import { aCashflow } from '@/flows/core/flows/__tests__/cashflow-for-test';
import { aTransaction } from '@/flows/core/flows/__tests__/transaction-for-test';
import { aStoreItem } from '@/shared/__tests__/store-item-for-test';
import { AuthenticationContextForTest } from '@/shared/auth/__tests__/AuthenticationContextForTest';
import { aSettings } from '@/shared/core/__tests__/settings-for-test';
import { SETTINGS_CODE } from '@/shared/core/settings';
import { DatabaseForTest, setupDatabaseForTest } from '@/shared/persistence/__tests__/database-for-test';
import { PowerSyncProviderForTest } from '@/shared/persistence/__tests__/PowerSyncProviderForTest';

const inDays = (days: number): DateOnly => withDate(today()).addDays(days).toDateOnly();

describe('usePublishBillsDueWidget', () => {
  let db: DatabaseForTest;
  let publisher: BillsDueWidgetPublisherInMemory;
  let signedInUser: User | undefined;
  const account = anAccount();
  const category = aCategory({ type: CategoryTypes.expense });
  const gainCategory = aCategory({ type: CategoryTypes.gain });

  const aBill = (effectiveDate: DateOnly, amount = 10, categoryId = category.id) =>
    aCashflow({
      accountId: account.id,
      categoryId: categoryId,
      effectiveDate: effectiveDate,
      amount: Money.valid(amount),
      intervalType: DateIntervalTypes.oneTime,
      frequency: 1,
    });

  const wrapper = ({ children }: { children: React.ReactNode }) => (
    <AuthenticationContextForTest overrides={{ user: signedInUser }}>
      <PowerSyncProviderForTest database={db}>{children}</PowerSyncProviderForTest>
    </AuthenticationContextForTest>
  );

  const createHook = async () => await renderHook(() => usePublishBillsDueWidget(publisher), { wrapper: wrapper });

  const lastSummary = () => publisher.published().at(-1)?.[0].summary;
  const counts = () => {
    const summary = lastSummary();
    return summary && { pastDue: summary.pastDue.count, dueToday: summary.dueToday.count };
  };

  beforeEach(async () => {
    db = await setupDatabaseForTest();
    publisher = BillsDueWidgetPublisherInMemory();
    signedInUser = aUser();

    const settings = aSettings({ effectiveDate: today(), intervalType: DateIntervalTypes.monthly, frequency: 1 });
    await aStoreItem({ code: SETTINGS_CODE, data: JSON.stringify(settings) }).store(db);
    await account.store(db);
    await category.store(db);
    await gainCategory.store(db);
  });

  afterEach(async () => {
    await act(async () => {
      await db.cleanupTestDb();
    });
  });

  it('should publish the bills past due and due today', async () => {
    await aBill(inDays(-1), 20).store(db);
    await aBill(today(), 12.5).store(db);
    await aBill(today(), 7.5).store(db);

    await createHook();

    await waitFor(() =>
      expect(lastSummary()).toEqual({
        pastDue: { count: 1, expenses: Money.valid(20), gains: Money.ZERO },
        dueToday: { count: 2, expenses: Money.valid(20), gains: Money.ZERO },
      })
    );
  });

  it('should total income apart from bills', async () => {
    await aBill(today(), 25).store(db);
    await aBill(today(), 1500, gainCategory.id).store(db);

    await createHook();

    await waitFor(() => expect(lastSummary()?.dueToday).toEqual({ count: 2, expenses: Money.valid(25), gains: Money.valid(1500) }));
  });

  it('should publish the spending and the projected net flow for the period', async () => {
    await aTransaction({ accountId: account.id, categoryId: category.id, date: today(), amount: Money.valid(40) }).store(db);
    await aBill(today(), 10).store(db);

    await createHook();

    await waitFor(() => expect(publisher.periods().at(-1)).toEqual({ spending: 40, projected: -50 }));
  });

  it('should not count a bill already paid', async () => {
    const paid = aBill(today());
    await paid.store(db);
    await aTransaction({ accountId: account.id, categoryId: category.id, cashflowId: paid.id, cashflowDate: today(), date: today() }).store(db);
    await aBill(inDays(-2)).store(db);

    await createHook();

    await waitFor(() => expect(counts()).toEqual({ pastDue: 1, dueToday: 0 }));
  });

  it('should publish again when the app becomes active', async () => {
    const handlers: ((state: AppStateStatus) => void)[] = [];
    jest.spyOn(AppState, 'addEventListener').mockImplementation((_, handler) => {
      handlers.push(handler);
      return { remove: () => {} };
    });
    await aBill(today()).store(db);
    await createHook();
    await waitFor(() => expect(counts()).toEqual({ pastDue: 0, dueToday: 1 }));
    const count = publisher.published().length;

    await act(async () => handlers.forEach((handler) => handler('active')));

    await waitFor(() => expect(publisher.published().length).toBeGreaterThan(count));
  });

  it('should log and keep running when the widget cannot be published', async () => {
    const errors: unknown[][] = [];
    Logger.setLoggerFactory(() => ({ debug: () => {}, info: () => {}, warn: () => {}, error: (...args: unknown[]) => errors.push(args) }));
    publisher.isFailing(['publish-bills-due-widget-failed']);

    await createHook();

    await waitFor(() => expect(errors).toContainEqual([expect.any(String), ['publish-bills-due-widget-failed']]));
    Logger.setLoggerFactory(loggerFactoryForNoop);
  });

  it('should clear the widget when signed out', async () => {
    signedInUser = undefined;

    await createHook();

    await waitFor(() => expect(publisher.clears()).toBeGreaterThan(0));
    expect(publisher.published()).toEqual([]);
  });

  it('should clear the widget when the user signs out', async () => {
    await aBill(today()).store(db);
    const hook = await createHook();
    await waitFor(() => expect(counts()).toEqual({ pastDue: 0, dueToday: 1 }));
    const clears = publisher.clears();

    signedInUser = undefined;
    await act(async () => hook.rerender({}));

    await waitFor(() => expect(publisher.clears()).toBeGreaterThan(clears));
  });

  it('should clear the widget when it stops running', async () => {
    await aBill(today()).store(db);
    const hook = await createHook();
    await waitFor(() => expect(counts()).toEqual({ pastDue: 0, dueToday: 1 }));
    const clears = publisher.clears();

    await act(async () => hook.unmount());

    await waitFor(() => expect(publisher.clears()).toBeGreaterThan(clears));
  });
});
