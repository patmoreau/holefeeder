import { Result } from '@holefeeder/shared/core';
import { BadgeSchedule } from './badge-schedule';

export const BadgeSchedulerErrors = {
  scheduleFailed: 'schedule-badges-failed',
};

export type BadgeScheduler = {
  schedule: (schedule: BadgeSchedule) => Promise<Result<void>>;
};
