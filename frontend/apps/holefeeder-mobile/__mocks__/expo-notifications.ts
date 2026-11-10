export const SchedulableTriggerInputTypes = {
  DATE: 'date',
  TIME_INTERVAL: 'timeInterval',
} as const;

export const getPermissionsAsync = jest.fn();
export const requestPermissionsAsync = jest.fn();
export const setBadgeCountAsync = jest.fn();
export const getAllScheduledNotificationsAsync = jest.fn();
export const cancelScheduledNotificationAsync = jest.fn();
export const scheduleNotificationAsync = jest.fn();
