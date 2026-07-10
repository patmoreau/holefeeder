import { DateOnly } from './date-only';
import { Result } from './result';
import { Validate, Validator } from './validate';
import { withDate } from './with-date';

export const DateIntervalTypes = {
  daily: 'daily',
  weekly: 'weekly',
  monthly: 'monthly',
  yearly: 'yearly',
  oneTime: 'oneTime',
} as const;

export type DateIntervalType = (typeof DateIntervalTypes)[keyof typeof DateIntervalTypes];

export const DateIntervalTypeErrors = {
  invalid: 'date-interval-type-invalid',
};

const isValid = Validator.enum<DateIntervalType>({ values: DateIntervalTypes });

export const normalizeDateIntervalType = (type: string): DateIntervalType => {
  const normalized = type.trim().toLowerCase();
  switch (normalized) {
    case 'daily':
      return DateIntervalTypes.daily;
    case 'weekly':
      return DateIntervalTypes.weekly;
    case 'monthly':
      return DateIntervalTypes.monthly;
    case 'yearly':
      return DateIntervalTypes.yearly;
    case 'onetime':
    case 'one_time':
    case 'one-time':
      return DateIntervalTypes.oneTime;
    default:
      return DateIntervalTypes.oneTime;
  }
};

const create = (value: unknown): Result<DateIntervalType> => {
  let normalized = value;
  if (typeof value === 'string') {
    const potential = normalizeDateIntervalType(value);
    if (potential !== DateIntervalTypes.oneTime) {
      normalized = potential;
    } else if (['onetime', 'one_time', 'one-time'].includes(value.trim().toLowerCase())) {
      normalized = potential;
    }
  }

  const result = Validate.validate(isValid, normalized, [DateIntervalTypeErrors.invalid]);
  if (result.isSuccess) {
    return Result.success(normalized as DateIntervalType);
  }
  return result;
};

const valid = (value: unknown): DateIntervalType => {
  return normalizeDateIntervalType(value as string);
};

const addIteration = (effectiveDate: DateOnly, iteration: number, intervalType: DateIntervalType): DateOnly => {
  switch (intervalType) {
    case DateIntervalTypes.daily:
      return withDate(effectiveDate).addDays(iteration).toDateOnly();
    case DateIntervalTypes.weekly:
      return withDate(effectiveDate).addWeeks(iteration).toDateOnly();
    case DateIntervalTypes.monthly:
      return withDate(effectiveDate).addMonths(iteration).toDateOnly();
    case DateIntervalTypes.yearly:
      return withDate(effectiveDate).addYears(iteration).toDateOnly();
    case DateIntervalTypes.oneTime:
    default:
      return effectiveDate;
  }
};

const interval = (
  effectiveDate: DateOnly,
  lookupDate: DateOnly,
  frequency: number,
  intervalType: DateIntervalType
): { from: DateOnly; to: DateOnly } => {
  if (intervalType === DateIntervalTypes.oneTime) {
    return { from: effectiveDate, to: effectiveDate };
  }

  const next = lookupDate > effectiveDate;
  let start = effectiveDate;
  let end = withDate(addIteration(start, frequency, intervalType))
    .addDays(-1)
    .toDateOnly();
  let iteration = 1;

  while (start > lookupDate || end < lookupDate) {
    start = addIteration(effectiveDate, (next ? frequency : -frequency) * iteration, intervalType);
    end = withDate(addIteration(start, frequency, intervalType))
      .addDays(-1)
      .toDateOnly();
    iteration++;
  }

  return { from: start, to: end };
};

const previousPeriods = (
  effectiveDate: DateOnly,
  intervalType: DateIntervalType,
  frequency: number,
  beforeDate: DateOnly
): { start: DateOnly; end: DateOnly }[] => {
  if (intervalType === DateIntervalTypes.oneTime) {
    return [];
  }

  const periods: { start: DateOnly; end: DateOnly }[] = [];
  let start = effectiveDate;
  let iteration = 1;

  // Walk forward from the anchor; a period is complete only when its exclusive
  // end (the next boundary) does not pass the current period start (beforeDate).
  for (;;) {
    const next = addIteration(effectiveDate, frequency * iteration, intervalType);
    if (next > beforeDate) {
      break;
    }
    periods.push({ start: start, end: next });
    start = next;
    iteration++;
  }

  return periods;
};

const datesInRange = (
  effectiveDate: DateOnly,
  fromDate: DateOnly,
  toDate: DateOnly,
  frequency: number,
  intervalType: DateIntervalType
): DateOnly[] => {
  if (intervalType === DateIntervalTypes.oneTime) {
    if (effectiveDate >= fromDate && effectiveDate <= toDate) {
      return [effectiveDate];
    }
    return [];
  }

  const dates: DateOnly[] = [];
  let start = effectiveDate;

  let iteration = 1;
  while (start < fromDate) {
    start = addIteration(effectiveDate, frequency * iteration, intervalType);
    iteration++;
  }

  while (start <= toDate) {
    dates.push(start);
    start = addIteration(effectiveDate, frequency * iteration, intervalType);
    iteration++;
  }

  return dates;
};

export const DateIntervalType = {
  create: create,
  valid: valid,
  addIteration: addIteration,
  interval: interval,
  datesInRange: datesInRange,
  previousPeriods: previousPeriods,
};
