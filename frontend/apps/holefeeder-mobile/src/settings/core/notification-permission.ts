import { Result } from '@holefeeder/shared/core';

export const NotificationPermissionStatuses = {
  granted: 'granted',
  denied: 'denied',
  undetermined: 'undetermined',
} as const;

export type NotificationPermissionStatus = (typeof NotificationPermissionStatuses)[keyof typeof NotificationPermissionStatuses];

export const NotificationPermissionErrors = {
  checkFailed: 'check-notification-permission-failed',
  requestFailed: 'request-notification-permission-failed',
  openSettingsFailed: 'open-notification-settings-failed',
};

export type NotificationPermission = {
  status: () => Promise<Result<NotificationPermissionStatus>>;
  request: () => Promise<Result<NotificationPermissionStatus>>;
  openSettings: () => Promise<Result<void>>;
};
