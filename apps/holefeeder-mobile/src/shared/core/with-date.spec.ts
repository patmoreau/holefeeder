import { DateOnly } from '@/shared/core/date-only';
import { today, withDate } from '@/shared/core/with-date';

describe('today', () => {
  it('returns the current date from startOfToday of date-fns', () => {
    jest.useFakeTimers().setSystemTime(new Date('2025-07-13T12:00:00Z'));

    expect(today()).toEqual('2025-07-13');
  });
});

describe('with-date', () => {
  it('adds days', () => {
    const date = DateOnly.valid('2025-07-13');
    expect(withDate(date).addDays(5).toDateOnly()).toBe('2025-07-18');
  });

  it('adds weeks', () => {
    const date = DateOnly.valid('2025-07-13');
    expect(withDate(date).addWeeks(1).toDateOnly()).toBe('2025-07-20');
  });

  it('adds months', () => {
    const date = DateOnly.valid('2025-07-13');
    expect(withDate(date).addMonths(2).toDateOnly()).toBe('2025-09-13');
  });

  it('adds months to february', () => {
    const date = DateOnly.valid('2025-01-30');
    expect(withDate(date).addMonths(1).toDateOnly()).toBe('2025-02-28');
  });

  it('adds years', () => {
    const date = DateOnly.valid('2025-07-13');
    expect(withDate(date).addYears(1).toDateOnly()).toBe('2026-07-13');
  });

  describe('toDateOnly', () => {
    it('accepts Date', () => {
      const date = new Date(2025, 6, 13);
      expect(withDate(date).toDateOnly()).toBe('2025-07-13');
    });

    it('accepts DateOnly', () => {
      const date = DateOnly.valid('2025-07-13');
      expect(withDate(date).toDateOnly()).toBe(date);
    });
  });

  describe('toDate', () => {
    it('accepts Date', () => {
      const date = new Date(2025, 6, 13);
      expect(withDate(date).toDate()).toBe(date);
    });

    it('accepts DateOnly', () => {
      const dateString = '2025-07-13';
      const date = DateOnly.valid(dateString);
      const expected = new Date(2025, 6, 13);
      expect(withDate(date).toDate()).toEqual(expected);
    });
  });
});
