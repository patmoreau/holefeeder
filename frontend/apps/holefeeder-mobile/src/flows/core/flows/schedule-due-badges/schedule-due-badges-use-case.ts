import { DateOnly, Result } from '@holefeeder/shared/core';
import { UpcomingFlow } from '@/flows/core/flows/upcoming-flow';
import { BadgeScheduler } from './badge-scheduler';

const MAX_SCHEDULED_DAYS = 64;

export const ScheduleDueBadgesUseCase = (scheduler: BadgeScheduler) => {
  const execute = async (flows: UpcomingFlow[], today: DateOnly): Promise<Result<void>> => {
    const dueBy = (date: DateOnly) => flows.filter((flow) => flow.date <= date).length;
    const futureDates = [...new Set(flows.map((flow) => flow.date).filter((date) => date > today))].sort().slice(0, MAX_SCHEDULED_DAYS);

    return await scheduler.schedule({
      today: dueBy(today),
      days: futureDates.map((date) => ({ date: date, count: dueBy(date) })),
    });
  };

  return {
    execute: execute,
  };
};
