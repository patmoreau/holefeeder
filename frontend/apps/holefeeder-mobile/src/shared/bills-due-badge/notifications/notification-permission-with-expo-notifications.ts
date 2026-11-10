import { Logger, Result } from '@holefeeder/shared/core';
import * as Notifications from 'expo-notifications';
import { Linking } from 'react-native';
import {
  NotificationPermission,
  NotificationPermissionErrors,
  NotificationPermissionStatus,
  NotificationPermissionStatuses,
} from '@/shared/bills-due-badge/core/notification-permission';

const logger = Logger.create('NotificationPermissionWithExpoNotifications');

const toStatus = (response: Notifications.NotificationPermissionsStatus): NotificationPermissionStatus => {
  if (response.status === 'undetermined') return NotificationPermissionStatuses.undetermined;
  if (response.status === 'granted' && response.ios?.allowsBadge !== false) return NotificationPermissionStatuses.granted;
  return NotificationPermissionStatuses.denied;
};

const attempt = async <T>(error: string, action: () => Promise<T>): Promise<Result<T>> => {
  try {
    return Result.success(await action());
  } catch (caught) {
    if (caught instanceof Error) {
      logger.error(`${error}: `, caught.message);
    }
    return Result.failure([error]);
  }
};

export const NotificationPermissionWithExpoNotifications = (): NotificationPermission => {
  const status = async (): Promise<Result<NotificationPermissionStatus>> =>
    attempt(NotificationPermissionErrors.checkFailed, async () => toStatus(await Notifications.getPermissionsAsync()));

  const request = async (): Promise<Result<NotificationPermissionStatus>> =>
    attempt(NotificationPermissionErrors.requestFailed, async () =>
      toStatus(await Notifications.requestPermissionsAsync({ ios: { allowAlert: false, allowBadge: true, allowSound: false } }))
    );

  const openSettings = async (): Promise<Result<void>> =>
    attempt(NotificationPermissionErrors.openSettingsFailed, () => Linking.openSettings());

  return {
    status: status,
    request: request,
    openSettings: openSettings,
  };
};
