import * as Notifications from 'expo-notifications';
import { Linking } from 'react-native';
import { NotificationPermissionErrors, NotificationPermissionStatuses } from '@/shared/bills-due-badge/core/notification-permission';
import { NotificationPermissionWithExpoNotifications } from '@/shared/bills-due-badge/notifications/notification-permission-with-expo-notifications';

const getPermissionsAsync = jest.mocked(Notifications.getPermissionsAsync);
const requestPermissionsAsync = jest.mocked(Notifications.requestPermissionsAsync);

const aPermissionResponse = (status: string, allowsBadge: boolean | null = true) =>
  ({
    status: status,
    granted: status === 'granted',
    canAskAgain: status !== 'denied',
    expires: 'never',
    ios: { allowsBadge: allowsBadge },
  }) as unknown as Notifications.NotificationPermissionsStatus;

describe('NotificationPermissionWithExpoNotifications', () => {
  let permission: ReturnType<typeof NotificationPermissionWithExpoNotifications>;

  beforeEach(() => {
    jest.clearAllMocks();
    permission = NotificationPermissionWithExpoNotifications();
  });

  it('should be granted when notifications and badges are allowed', async () => {
    getPermissionsAsync.mockResolvedValue(aPermissionResponse('granted'));

    const result = await permission.status();

    expect(result).toBeSuccessWithValue(NotificationPermissionStatuses.granted);
  });

  it('should be denied when notifications are allowed but badges are turned off', async () => {
    getPermissionsAsync.mockResolvedValue(aPermissionResponse('granted', false));

    const result = await permission.status();

    expect(result).toBeSuccessWithValue(NotificationPermissionStatuses.denied);
  });

  it('should be denied when notifications are denied', async () => {
    getPermissionsAsync.mockResolvedValue(aPermissionResponse('denied', null));

    const result = await permission.status();

    expect(result).toBeSuccessWithValue(NotificationPermissionStatuses.denied);
  });

  it('should be undetermined when notifications were never requested', async () => {
    getPermissionsAsync.mockResolvedValue(aPermissionResponse('undetermined', null));

    const result = await permission.status();

    expect(result).toBeSuccessWithValue(NotificationPermissionStatuses.undetermined);
  });

  it('should request badges only', async () => {
    requestPermissionsAsync.mockResolvedValue(aPermissionResponse('granted'));

    const result = await permission.request();

    expect(result).toBeSuccessWithValue(NotificationPermissionStatuses.granted);
    expect(requestPermissionsAsync).toHaveBeenCalledWith({ ios: { allowAlert: false, allowBadge: true, allowSound: false } });
  });

  it('should open the app settings', async () => {
    const openSettings = jest.spyOn(Linking, 'openSettings').mockResolvedValue(undefined);

    const result = await permission.openSettings();

    expect(result).toBeSuccessWithValue(undefined);
    expect(openSettings).toHaveBeenCalled();
  });

  it('should fail when the permission cannot be checked', async () => {
    getPermissionsAsync.mockRejectedValue(new Error('unavailable'));

    const result = await permission.status();

    expect(result).toBeFailureWithErrors([NotificationPermissionErrors.checkFailed]);
  });

  it('should fail when the permission cannot be requested', async () => {
    requestPermissionsAsync.mockRejectedValue(new Error('unavailable'));

    const result = await permission.request();

    expect(result).toBeFailureWithErrors([NotificationPermissionErrors.requestFailed]);
  });

  it('should fail when the app settings cannot be opened', async () => {
    jest.spyOn(Linking, 'openSettings').mockRejectedValue(new Error('unavailable'));

    const result = await permission.openSettings();

    expect(result).toBeFailureWithErrors([NotificationPermissionErrors.openSettingsFailed]);
  });
});
