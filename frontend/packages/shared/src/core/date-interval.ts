import { DateIntervalType } from './date-interval-type';
import { DateOnly } from './date-only';
import { Result } from './result';

export type DateInterval = {
  start: DateOnly;
  end: DateOnly;
};

export const DateIntervalErrors = {
  invalid: 'date-interval-invalid',
};

const create = (start: DateOnly, end: DateOnly): Result<DateInterval> => {
  if (start > end) {
    return Result.failure([DateIntervalErrors.invalid]);
  }

  return Result.success({ start, end });
};

const createFrom = (
  asOfDate: DateOnly,
  iteration: number,
  effectiveDate: DateOnly,
  intervalType: DateIntervalType,
  frequency: number
): Result<DateInterval> => {
  const startDate = DateIntervalType.addIteration(asOfDate, iteration * frequency, intervalType);

  const interval = DateIntervalType.interval(effectiveDate, startDate, frequency, intervalType);

  return create(interval.from, interval.to);
};

export const DateInterval = {
  create,
  createFrom,
} as const;
