import { DateOnly } from '@holefeeder/shared/core';
import * as Notifications from 'expo-notifications';
import { BadgeSchedulerErrors } from '@/flows/core/flows/schedule-due-badges/badge-scheduler';
import { BadgeSchedulerWithExpoNotifications } from '@/flows/notifications/badge-scheduler-with-expo-notifications';

const setBadgeCountAsync = jest.mocked(Notifications.setBadgeCountAsync);
const getAllScheduledNotificationsAsync = jest.mocked(Notifications.getAllScheduledNotificationsAsync);
const cancelScheduledNotificationAsync = jest.mocked(Notifications.cancelScheduledNotificationAsync);
const scheduleNotificationAsync = jest.mocked(Notifications.scheduleNotificationAsync);

describe('BadgeSchedulerWithExpoNotifications', () => {
  let scheduler: ReturnType<typeof BadgeSchedulerWithExpoNotifications>;

  beforeEach(() => {
    jest.clearAllMocks();
    setBadgeCountAsync.mockResolvedValue(true);
    getAllScheduledNotificationsAsync.mockResolvedValue([]);
    cancelScheduledNotificationAsync.mockResolvedValue(undefined);
    scheduleNotificationAsync.mockImplementation(async (request) => request.identifier ?? '');
    scheduler = BadgeSchedulerWithExpoNotifications();
  });

  it('should set the app badge to the count due today', async () => {
    await scheduler.schedule({ today: 2, days: [] });

    expect(setBadgeCountAsync).toHaveBeenCalledWith(2);
  });

  it('should schedule a badge-only notification at the start of each day', async () => {
    await scheduler.schedule({
      today: 0,
      days: [
        { date: DateOnly.valid('2026-11-10'), count: 1 },
        { date: DateOnly.valid('2026-11-14'), count: 3 },
      ],
    });

    expect(scheduleNotificationAsync.mock.calls).toEqual([
      [
        {
          identifier: 'due-badge-2026-11-10',
          content: { badge: 1 },
          trigger: { type: Notifications.SchedulableTriggerInputTypes.DATE, date: new Date(2026, 10, 10) },
        },
      ],
      [
        {
          identifier: 'due-badge-2026-11-14',
          content: { badge: 3 },
          trigger: { type: Notifications.SchedulableTriggerInputTypes.DATE, date: new Date(2026, 10, 14) },
        },
      ],
    ]);
  });

  it('should cancel pending due badges before scheduling new ones', async () => {
    getAllScheduledNotificationsAsync.mockResolvedValue([
      aScheduledNotification('due-badge-2026-11-10'),
      aScheduledNotification('another-notification'),
      aScheduledNotification('due-badge-2026-11-12'),
    ]);

    await scheduler.schedule({ today: 0, days: [{ date: DateOnly.valid('2026-11-11'), count: 1 }] });

    expect(cancelScheduledNotificationAsync.mock.calls).toEqual([['due-badge-2026-11-10'], ['due-badge-2026-11-12']]);
    expect(cancelScheduledNotificationAsync.mock.invocationCallOrder[1]).toBeLessThan(scheduleNotificationAsync.mock.invocationCallOrder[0]);
  });

  it('should clear the badge and pending due badges when nothing is due', async () => {
    getAllScheduledNotificationsAsync.mockResolvedValue([aScheduledNotification('due-badge-2026-11-10')]);

    const result = await scheduler.schedule({ today: 0, days: [] });

    expect(result).toBeSuccessWithValue(undefined);
    expect(setBadgeCountAsync).toHaveBeenCalledWith(0);
    expect(cancelScheduledNotificationAsync).toHaveBeenCalledWith('due-badge-2026-11-10');
    expect(scheduleNotificationAsync).not.toHaveBeenCalled();
  });

  it('should fail when notifications cannot be scheduled', async () => {
    scheduleNotificationAsync.mockRejectedValue(new Error('denied'));

    const result = await scheduler.schedule({ today: 0, days: [{ date: DateOnly.valid('2026-11-10'), count: 1 }] });

    expect(result).toBeFailureWithErrors([BadgeSchedulerErrors.scheduleFailed]);
  });
});

const aScheduledNotification = (identifier: string) =>
  ({ identifier: identifier, content: {}, trigger: null }) as unknown as Notifications.NotificationRequest;
