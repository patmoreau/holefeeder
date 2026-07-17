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

const daysBetween = (from: DateOnly, to: DateOnly): number => {
  const [fy, fm, fd] = from.split('-').map(Number);
  const [ty, tm, td] = to.split('-').map(Number);
  return Math.round((Date.UTC(ty, tm - 1, td) - Date.UTC(fy, fm - 1, fd)) / 86_400_000);
};

// Approximate how many `frequency`-sized steps separate the anchor from the lookup date.
// This only needs to be close: the correction loops in `interval` walk from here to the exact
// bracketing period, so an off-by-a-few estimate stays correct — it just avoids the O(distance)
// walk from the anchor that is slow when the anchor is many years before the lookup date.
const approximateSteps = (effectiveDate: DateOnly, lookupDate: DateOnly, frequency: number, intervalType: DateIntervalType): number => {
  const [effYear, effMonth] = effectiveDate.split('-').map(Number);
  const [lookYear, lookMonth] = lookupDate.split('-').map(Number);

  switch (intervalType) {
    case DateIntervalTypes.daily:
      return Math.floor(daysBetween(effectiveDate, lookupDate) / frequency);
    case DateIntervalTypes.weekly:
      return Math.floor(daysBetween(effectiveDate, lookupDate) / (7 * frequency));
    case DateIntervalTypes.monthly:
      return Math.floor(((lookYear - effYear) * 12 + (lookMonth - effMonth)) / frequency);
    case DateIntervalTypes.yearly:
      return Math.floor((lookYear - effYear) / frequency);
    default:
      return 0;
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

  const periodFrom = (steps: number): DateOnly => addIteration(effectiveDate, steps * frequency, intervalType);
  const periodEnd = (from: DateOnly): DateOnly =>
    withDate(addIteration(from, frequency, intervalType))
      .addDays(-1)
      .toDateOnly();

  // Seed close to the answer, then correct one step at a time. Periods tile contiguously on the
  // anchored grid (each end is the day before the next start), so exactly one of these loops runs,
  // and only for a couple of iterations regardless of how far the lookup date is from the anchor.
  let steps = approximateSteps(effectiveDate, lookupDate, frequency, intervalType);
  let from = periodFrom(steps);
  let to = periodEnd(from);

  while (from > lookupDate) {
    steps -= 1;
    from = periodFrom(steps);
    to = periodEnd(from);
  }
  while (to < lookupDate) {
    steps += 1;
    from = periodFrom(steps);
    to = periodEnd(from);
  }

  return { from, to };
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
