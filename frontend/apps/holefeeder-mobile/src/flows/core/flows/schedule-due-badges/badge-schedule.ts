import { DateOnly } from '@holefeeder/shared/core';

export type DueBadge = {
  date: DateOnly;
  count: number;
};

export type BadgeSchedule = {
  today: number;
  days: DueBadge[];
};
