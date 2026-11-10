import { Result } from '@holefeeder/shared/core';
import { BadgeSchedule } from './badge-schedule';

export type BadgeScheduler = {
  schedule: (schedule: BadgeSchedule) => Promise<Result<void>>;
};
