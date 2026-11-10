import { act, renderHook, RenderHookResult, waitFor } from '@testing-library/react-native';
import React from 'react';
import { AppState, AppStateStatus } from 'react-native';
import { AppStorageInMemory } from '@/shared/__tests__/app-storage-in-memory';
import { NotificationPermissionInMemory } from '@/shared/bills-due-badge/__tests__/notification-permission-in-memory';
import { BillsDueBadgeState } from '@/shared/bills-due-badge/core/bills-due-badge-state';
import { NotificationPermissionStatuses } from '@/shared/bills-due-badge/core/notification-permission';
import { useBillsDueBadge } from '@/shared/bills-due-badge/core/use-bills-due-badge';
import { BillsDueBadgeProvider } from '@/shared/bills-due-badge/presentation/BillsDueBadgeProvider';

const BILLS_DUE_BADGE_KEY = 'app_settings_bills_due_badge';

describe('useBillsDueBadge', () => {
  let storage: AppStorageInMemory;
  let permission: NotificationPermissionInMemory;

  beforeEach(() => {
    storage = AppStorageInMemory();
    permission = NotificationPermissionInMemory();
  });

  const createHook = async (): Promise<RenderHookResult<BillsDueBadgeState, unknown>> =>
    await waitFor(() =>
      renderHook(() => useBillsDueBadge(), {
        wrapper: ({ children }: { children: React.ReactNode }) => (
          <BillsDueBadgeProvider storage={storage} permission={permission}>
            {children}
          </BillsDueBadgeProvider>
        ),
      })
    );

  it('should be off when nothing is saved', async () => {
    const hook = await createHook();

    expect(hook.result.current.enabled).toBe(false);
    expect(hook.result.current.denied).toBe(false);
  });

  it('should be on when saved on and badges are allowed', async () => {
    storage.setString(BILLS_DUE_BADGE_KEY, 'true');
    permission.withStatus(NotificationPermissionStatuses.granted);

    const hook = await createHook();

    await waitFor(() => expect(hook.result.current.enabled).toBe(true));
    expect(hook.result.current.denied).toBe(false);
  });

  it('should be off and denied when saved on but badges are denied', async () => {
    storage.setString(BILLS_DUE_BADGE_KEY, 'true');
    permission.withStatus(NotificationPermissionStatuses.denied);

    const hook = await createHook();

    await waitFor(() => expect(hook.result.current.denied).toBe(true));
    expect(hook.result.current.enabled).toBe(false);
  });

  it('should turn on without asking when badges are already allowed', async () => {
    permission.withStatus(NotificationPermissionStatuses.granted);
    const hook = await createHook();

    await act(async () => hook.result.current.setEnabled(true));

    expect(hook.result.current.enabled).toBe(true);
    expect(storage.getString(BILLS_DUE_BADGE_KEY)).toBe('true');
    expect(permission.requestCount()).toBe(0);
  });

  it('should ask for badges and turn on when allowed', async () => {
    permission.answersRequestWith(NotificationPermissionStatuses.granted);
    const hook = await createHook();

    await act(async () => hook.result.current.setEnabled(true));

    expect(permission.requestCount()).toBe(1);
    expect(hook.result.current.enabled).toBe(true);
    expect(storage.getString(BILLS_DUE_BADGE_KEY)).toBe('true');
  });

  it('should stay off and show denied when the request is refused', async () => {
    permission.answersRequestWith(NotificationPermissionStatuses.denied);
    const hook = await createHook();

    await act(async () => hook.result.current.setEnabled(true));

    expect(hook.result.current.enabled).toBe(false);
    expect(hook.result.current.denied).toBe(true);
    expect(storage.getString(BILLS_DUE_BADGE_KEY)).not.toBe('true');
  });

  it('should turn off without touching the permission', async () => {
    storage.setString(BILLS_DUE_BADGE_KEY, 'true');
    permission.withStatus(NotificationPermissionStatuses.granted);
    const hook = await createHook();
    await waitFor(() => expect(hook.result.current.enabled).toBe(true));

    await act(async () => hook.result.current.setEnabled(false));

    expect(hook.result.current.enabled).toBe(false);
    expect(storage.getString(BILLS_DUE_BADGE_KEY)).toBe('false');
    expect(permission.requestCount()).toBe(0);
  });

  it('should be off and not denied when the permission cannot be checked', async () => {
    storage.setString(BILLS_DUE_BADGE_KEY, 'true');
    permission.isFailing();

    const hook = await createHook();

    expect(hook.result.current.enabled).toBe(false);
    expect(hook.result.current.denied).toBe(false);
  });

  it('should open the app settings', async () => {
    const hook = await createHook();

    await act(async () => hook.result.current.openSettings());

    expect(permission.openSettingsCount()).toBe(1);
  });

  it('should check the permission again when the app becomes active', async () => {
    let onChange: (state: AppStateStatus) => void = () => {};
    jest.spyOn(AppState, 'addEventListener').mockImplementation((_, handler) => {
      onChange = handler;
      return { remove: () => {} };
    });
    storage.setString(BILLS_DUE_BADGE_KEY, 'true');
    permission.withStatus(NotificationPermissionStatuses.denied);
    const hook = await createHook();
    await waitFor(() => expect(hook.result.current.denied).toBe(true));

    permission.withStatus(NotificationPermissionStatuses.granted);
    await act(async () => onChange('active'));

    await waitFor(() => expect(hook.result.current.enabled).toBe(true));
    expect(hook.result.current.denied).toBe(false);
  });
});
