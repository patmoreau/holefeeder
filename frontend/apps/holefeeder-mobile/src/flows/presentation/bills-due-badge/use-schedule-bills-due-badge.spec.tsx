import { CategoryTypes, DateIntervalTypes, DateOnly, Logger, loggerFactoryForNoop, today, User, withDate } from '@holefeeder/shared/core';
import { aUser, aSettings } from '@holefeeder/shared/testkit';
import { act, renderHook, waitFor } from '@testing-library/react-native';
import React from 'react';
import { AppState, AppStateStatus } from 'react-native';
import { anAccount } from '@/accounts/core/__tests__/account-for-test';
import { aCategory } from '@/flows/core/categories/__tests__/category-for-test';
import { BadgeSchedulerInMemory } from '@/flows/core/flows/__tests__/badge-scheduler-in-memory';
import { aCashflow } from '@/flows/core/flows/__tests__/cashflow-for-test';
import { aTransaction } from '@/flows/core/flows/__tests__/transaction-for-test';
import { useScheduleBillsDueBadge } from '@/flows/presentation/bills-due-badge/use-schedule-bills-due-badge';
import { AppStorageInMemory } from '@/shared/__tests__/app-storage-in-memory';
import { aStoreItem } from '@/shared/__tests__/store-item-for-test';
import { AuthenticationContextForTest } from '@/shared/auth/__tests__/AuthenticationContextForTest';
import { NotificationPermissionInMemory } from '@/shared/bills-due-badge/__tests__/notification-permission-in-memory';
import { NotificationPermissionStatuses } from '@/shared/bills-due-badge/core/notification-permission';
import { useBillsDueBadge } from '@/shared/bills-due-badge/core/use-bills-due-badge';
import { BillsDueBadgeProvider } from '@/shared/bills-due-badge/presentation/BillsDueBadgeProvider';
import { SETTINGS_CODE } from '@/shared/core/settings-code';
import { DatabaseForTest, setupDatabaseForTest } from '@/shared/persistence/__tests__/database-for-test';
import { PowerSyncProviderForTest } from '@/shared/persistence/__tests__/PowerSyncProviderForTest';

const BILLS_DUE_BADGE_KEY = 'app_settings_bills_due_badge';

const inDays = (days: number): DateOnly => withDate(today()).addDays(days).toDateOnly();

describe('useScheduleBillsDueBadge', () => {
  let db: DatabaseForTest;
  let storage: AppStorageInMemory;
  let permission: NotificationPermissionInMemory;
  let scheduler: BadgeSchedulerInMemory;
  let signedInUser: User | undefined;
  const account = anAccount();
  const category = aCategory({ type: CategoryTypes.expense });

  const aBill = (effectiveDate: DateOnly, intervalType = DateIntervalTypes.oneTime) =>
    aCashflow({ accountId: account.id, categoryId: category.id, effectiveDate: effectiveDate, intervalType: intervalType, frequency: 1 });

  const wrapper = ({ children }: { children: React.ReactNode }) => (
    <AuthenticationContextForTest overrides={{ user: signedInUser }}>
      <PowerSyncProviderForTest database={db}>
        <BillsDueBadgeProvider storage={storage} permission={permission}>
          {children}
        </BillsDueBadgeProvider>
      </PowerSyncProviderForTest>
    </AuthenticationContextForTest>
  );

  const createHook = async () => await renderHook(() => useScheduleBillsDueBadge(scheduler), { wrapper: wrapper });

  const lastScheduled = () => scheduler.scheduled().at(-1);

  beforeEach(async () => {
    db = await setupDatabaseForTest();
    storage = AppStorageInMemory();
    permission = NotificationPermissionInMemory();
    permission.withStatus(NotificationPermissionStatuses.granted);
    scheduler = BadgeSchedulerInMemory();
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

  it('should schedule the bills due today and on later days when switched on', async () => {
    storage.setString(BILLS_DUE_BADGE_KEY, 'true');
    await aBill(today()).store(db);
    await aBill(inDays(3)).store(db);

    await createHook();

    await waitFor(() => expect(lastScheduled()).toEqual({ today: 1, days: [{ date: inDays(3), count: 2 }] }));
  });

  it('should count an unpaid bill from yesterday today', async () => {
    storage.setString(BILLS_DUE_BADGE_KEY, 'true');
    await aBill(inDays(-1)).store(db);

    await createHook();

    await waitFor(() => expect(lastScheduled()).toEqual({ today: 1, days: [] }));
  });

  it('should not count a bill already paid', async () => {
    storage.setString(BILLS_DUE_BADGE_KEY, 'true');
    const paid = aBill(today());
    await paid.store(db);
    await aTransaction({ accountId: account.id, categoryId: category.id, cashflowId: paid.id, cashflowDate: today(), date: today() }).store(db);
    await aBill(today()).store(db);

    await createHook();

    await waitFor(() => expect(lastScheduled()).toEqual({ today: 1, days: [] }));
  });

  it('should never schedule from bills that are still loading', async () => {
    storage.setString(BILLS_DUE_BADGE_KEY, 'true');
    await aBill(today()).store(db);

    await createHook();

    await waitFor(() => expect(lastScheduled()).toBeDefined());
    expect(scheduler.scheduled()).toEqual(scheduler.scheduled().map(() => ({ today: 1, days: [] })));
  });

  it('should clear the badge when switched off', async () => {
    await aBill(today()).store(db);

    await createHook();

    await waitFor(() => expect(lastScheduled()).toEqual({ today: 0, days: [] }));
  });

  it('should schedule the bills once switched on', async () => {
    await aBill(today()).store(db);
    const { result } = await renderHook(
      () => {
        useScheduleBillsDueBadge(scheduler);
        return useBillsDueBadge();
      },
      { wrapper: wrapper }
    );
    await waitFor(() => expect(lastScheduled()).toEqual({ today: 0, days: [] }));

    await act(async () => result.current.setEnabled(true));

    await waitFor(() => expect(lastScheduled()).toEqual({ today: 1, days: [] }));
  });

  it('should schedule again when the app becomes active', async () => {
    const handlers: ((state: AppStateStatus) => void)[] = [];
    jest.spyOn(AppState, 'addEventListener').mockImplementation((_, handler) => {
      handlers.push(handler);
      return { remove: () => {} };
    });
    storage.setString(BILLS_DUE_BADGE_KEY, 'true');
    await aBill(today()).store(db);
    await createHook();
    await waitFor(() => expect(lastScheduled()).toEqual({ today: 1, days: [] }));
    const count = scheduler.scheduled().length;

    await act(async () => handlers.forEach((handler) => handler('active')));

    await waitFor(() => expect(scheduler.scheduled().length).toBeGreaterThan(count));
    expect(lastScheduled()).toEqual({ today: 1, days: [] });
  });

  it('should log and keep running when the badge cannot be scheduled', async () => {
    const errors: unknown[][] = [];
    Logger.setLoggerFactory(() => ({ debug: () => {}, info: () => {}, warn: () => {}, error: (...args: unknown[]) => errors.push(args) }));
    storage.setString(BILLS_DUE_BADGE_KEY, 'true');
    scheduler.isFailing(['schedule-badges-failed']);
    await aBill(today()).store(db);

    await createHook();

    await waitFor(() => expect(errors).toContainEqual([expect.any(String), ['schedule-badges-failed']]));
    Logger.setLoggerFactory(loggerFactoryForNoop);
  });

  it('should clear the badge when the user signs out', async () => {
    storage.setString(BILLS_DUE_BADGE_KEY, 'true');
    await aBill(today()).store(db);
    const hook = await createHook();
    await waitFor(() => expect(lastScheduled()).toEqual({ today: 1, days: [] }));

    signedInUser = undefined;
    await act(async () => hook.rerender({}));

    await waitFor(() => expect(lastScheduled()).toEqual({ today: 0, days: [] }));
  });

  it('should clear the badge when it stops running', async () => {
    storage.setString(BILLS_DUE_BADGE_KEY, 'true');
    await aBill(today()).store(db);
    const hook = await createHook();
    await waitFor(() => expect(lastScheduled()).toEqual({ today: 1, days: [] }));

    await act(async () => hook.unmount());

    await waitFor(() => expect(lastScheduled()).toEqual({ today: 0, days: [] }));
  });

  it('should schedule the bills again when the user signs back in', async () => {
    storage.setString(BILLS_DUE_BADGE_KEY, 'true');
    await aBill(today()).store(db);
    const hook = await createHook();
    await waitFor(() => expect(lastScheduled()).toEqual({ today: 1, days: [] }));
    signedInUser = undefined;
    await act(async () => hook.rerender({}));
    await waitFor(() => expect(lastScheduled()).toEqual({ today: 0, days: [] }));

    signedInUser = aUser();
    await act(async () => hook.rerender({}));

    await waitFor(() => expect(lastScheduled()).toEqual({ today: 1, days: [] }));
  });
});
