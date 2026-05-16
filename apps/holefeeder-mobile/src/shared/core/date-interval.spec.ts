import { DateIntervalTypes } from '@/shared/core/date-interval-type';
import { DateOnly } from '@/shared/core/date-only';
import { DateInterval, DateIntervalErrors } from './date-interval';

describe('DateInterval', () => {
  describe('create', () => {
    it('should create a valid date interval', () => {
      const start = DateOnly.valid('2023-01-01');
      const end = DateOnly.valid('2023-01-31');

      const result = DateInterval.create(start, end);

      expect(result).toBeSuccessWithValue({ start, end });
    });

    it('should fail if end date is before start date', () => {
      const start = DateOnly.valid('2023-01-31');
      const end = DateOnly.valid('2023-01-01');

      const result = DateInterval.create(start, end);

      expect(result).toBeFailureWithErrors([DateIntervalErrors.invalid]);
    });
  });

  describe('createFrom', () => {
    const effectiveDate = DateOnly.valid('2023-01-01');

    it('should create a valid weekly interval', () => {
      const asOfDate = DateOnly.valid('2023-01-01');
      const result = DateInterval.createFrom(asOfDate, 0, effectiveDate, DateIntervalTypes.weekly, 1);

      expect(result).toBeSuccessWithValue({ start: DateOnly.valid('2023-01-01'), end: DateOnly.valid('2023-01-07') });
    });

    it('should create a valid weekly interval for next iteration', () => {
      const asOfDate = DateOnly.valid('2023-01-01');
      const result = DateInterval.createFrom(asOfDate, 1, effectiveDate, DateIntervalTypes.weekly, 1);

      expect(result).toBeSuccessWithValue({ start: DateOnly.valid('2023-01-08'), end: DateOnly.valid('2023-01-14') });
    });

    it('should create a valid monthly interval', () => {
      const asOfDate = DateOnly.valid('2023-01-01');
      const result = DateInterval.createFrom(asOfDate, 0, effectiveDate, DateIntervalTypes.monthly, 1);

      expect(result).toBeSuccessWithValue({ start: DateOnly.valid('2023-01-01'), end: DateOnly.valid('2023-01-31') });
    });

    it('should create a valid monthly interval for next iteration', () => {
      const asOfDate = DateOnly.valid('2023-01-01');
      const result = DateInterval.createFrom(asOfDate, 1, effectiveDate, DateIntervalTypes.monthly, 1);

      expect(result).toBeSuccessWithValue({ start: DateOnly.valid('2023-02-01'), end: DateOnly.valid('2023-02-28') });
    });

    it('should create a valid yearly interval', () => {
      const asOfDate = DateOnly.valid('2023-01-01');
      const result = DateInterval.createFrom(asOfDate, 0, effectiveDate, DateIntervalTypes.yearly, 1);

      expect(result).toBeSuccessWithValue({ start: DateOnly.valid('2023-01-01'), end: DateOnly.valid('2023-12-31') });
    });

    it('should create a valid yearly interval for next iteration', () => {
      const asOfDate = DateOnly.valid('2023-01-01');
      const result = DateInterval.createFrom(asOfDate, 1, effectiveDate, DateIntervalTypes.yearly, 1);

      expect(result).toBeSuccessWithValue({ start: DateOnly.valid('2024-01-01'), end: DateOnly.valid('2024-12-31') });
    });

    it('should create a valid interval with frequency > 1', () => {
      const asOfDate = DateOnly.valid('2023-01-01');
      const result = DateInterval.createFrom(asOfDate, 1, effectiveDate, DateIntervalTypes.weekly, 2);

      // iteration 0: 2023-01-01 to 2023-01-14
      // iteration 1: 2023-01-15 to 2023-01-28 (asOf + 1*2 weeks = 2023-01-15)

      expect(result).toBeSuccessWithValue({ start: DateOnly.valid('2023-01-15'), end: DateOnly.valid('2023-01-28') });
    });

    it('should handle oneTime interval', () => {
      const asOfDate = DateOnly.valid('2023-01-01');
      const result = DateInterval.createFrom(asOfDate, 0, effectiveDate, DateIntervalTypes.oneTime, 1);

      expect(result).toBeSuccessWithValue({ start: DateOnly.valid('2023-01-01'), end: DateOnly.valid('2023-01-01') });
    });
  });
});
