import { Result } from '@holefeeder/shared/core';
import {
  NotificationPermission,
  NotificationPermissionErrors,
  NotificationPermissionStatus,
  NotificationPermissionStatuses,
} from '@/shared/bills-due-badge/core/notification-permission';

export type NotificationPermissionInMemory = NotificationPermission & {
  withStatus: (status: NotificationPermissionStatus) => void;
  answersRequestWith: (status: NotificationPermissionStatus) => void;
  isFailing: () => void;
  requestCount: () => number;
  openSettingsCount: () => number;
};

export const NotificationPermissionInMemory = (): NotificationPermissionInMemory => {
  let statusInMemory: NotificationPermissionStatus = NotificationPermissionStatuses.undetermined;
  let answerInMemory: NotificationPermissionStatus = NotificationPermissionStatuses.granted;
  let failing = false;
  let requests = 0;
  let openSettingsCalls = 0;

  const status = async (): Promise<Result<NotificationPermissionStatus>> =>
    failing ? Result.failure([NotificationPermissionErrors.checkFailed]) : Result.success(statusInMemory);

  const request = async (): Promise<Result<NotificationPermissionStatus>> => {
    requests++;
    if (failing) return Result.failure([NotificationPermissionErrors.requestFailed]);
    statusInMemory = answerInMemory;
    return Result.success(statusInMemory);
  };

  const openSettings = async (): Promise<Result<void>> => {
    openSettingsCalls++;
    return Result.success();
  };

  return {
    status: status,
    request: request,
    openSettings: openSettings,
    withStatus: (value: NotificationPermissionStatus) => (statusInMemory = value),
    answersRequestWith: (value: NotificationPermissionStatus) => (answerInMemory = value),
    isFailing: () => (failing = true),
    requestCount: () => requests,
    openSettingsCount: () => openSettingsCalls,
  };
};
