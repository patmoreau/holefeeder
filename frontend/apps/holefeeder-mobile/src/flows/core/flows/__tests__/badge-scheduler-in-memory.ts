import { Result } from '@holefeeder/shared/core';
import { BadgeSchedule } from '@/flows/core/flows/schedule-due-badges/badge-schedule';
import { BadgeScheduler } from '@/flows/core/flows/schedule-due-badges/badge-scheduler';

export type BadgeSchedulerInMemory = BadgeScheduler & {
  isFailing: (errors: string[]) => void;
  scheduled: () => BadgeSchedule[];
};

export const BadgeSchedulerInMemory = (): BadgeSchedulerInMemory => {
  const scheduledInMemory: BadgeSchedule[] = [];
  let errorsInMemory: string[] = [];

  const schedule = async (badgeSchedule: BadgeSchedule): Promise<Result<void>> => {
    if (errorsInMemory.length > 0) return Result.failure(errorsInMemory);
    scheduledInMemory.push(badgeSchedule);
    return Result.success();
  };

  return {
    schedule: schedule,
    isFailing: (errors: string[]) => (errorsInMemory = errors),
    scheduled: () => [...scheduledInMemory],
  };
};
