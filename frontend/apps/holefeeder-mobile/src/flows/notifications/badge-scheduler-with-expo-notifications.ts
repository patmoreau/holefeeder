import { Logger, Result, withDate } from '@holefeeder/shared/core';
import * as Notifications from 'expo-notifications';
import { BadgeSchedule } from '@/flows/core/flows/schedule-due-badges/badge-schedule';
import { BadgeScheduler, BadgeSchedulerErrors } from '@/flows/core/flows/schedule-due-badges/badge-scheduler';

const IDENTIFIER_PREFIX = 'due-badge-';

const logger = Logger.create('BadgeSchedulerWithExpoNotifications');

export const BadgeSchedulerWithExpoNotifications = (): BadgeScheduler => {
  const cancelPendingDueBadges = async () => {
    const pending = await Notifications.getAllScheduledNotificationsAsync();
    for (const request of pending.filter((request) => request.identifier.startsWith(IDENTIFIER_PREFIX))) {
      await Notifications.cancelScheduledNotificationAsync(request.identifier);
    }
  };

  const schedule = async (badgeSchedule: BadgeSchedule): Promise<Result<void>> => {
    try {
      await Notifications.setBadgeCountAsync(badgeSchedule.today);
      await cancelPendingDueBadges();
      for (const day of badgeSchedule.days) {
        await Notifications.scheduleNotificationAsync({
          identifier: `${IDENTIFIER_PREFIX}${day.date}`,
          content: { badge: day.count },
          trigger: { type: Notifications.SchedulableTriggerInputTypes.DATE, date: withDate(day.date).toDate() },
        });
      }
      return Result.success();
    } catch (error) {
      if (error instanceof Error) {
        logger.error(`${BadgeSchedulerErrors.scheduleFailed}: `, error.message);
      }
      return Result.failure([BadgeSchedulerErrors.scheduleFailed]);
    }
  };

  return {
    schedule: schedule,
  };
};
