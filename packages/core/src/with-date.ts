import { addDays, addMonths, addWeeks, addYears, format, parseISO, startOfToday } from 'date-fns';
import { DateOnly } from './date-only';

export const today = () => withDate(startOfToday()).toDateOnly();

export const withDate = (date: Date | DateOnly) => {
  let current: Date = typeof date === 'string' ? parseISO(date) : date;

  return {
    addDays: (amount: number) => withDate(addDays(current, amount)),
    addWeeks: (amount: number) => withDate(addWeeks(current, amount)),
    addMonths: (amount: number) => withDate(addMonths(current, amount)),
    addYears: (amount: number) => withDate(addYears(current, amount)),

    toDateOnly: (): DateOnly => DateOnly.valid(format(current, 'yyyy-MM-dd')),

    toDate: (): Date => current,
  };
};
