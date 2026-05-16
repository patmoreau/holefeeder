import { DateOnly, DateOnlyErrors } from '@/shared/core/date-only';

describe('DateOnly', () => {
  describe('create', () => {
    it('should create a Date value for valid YYYY-MM-DD format', () => {
      expect(DateOnly.create('2026-01-24')).toBeSuccessWithValue('2026-01-24');
    });

    it('should create a Date value for first day of year', () => {
      expect(DateOnly.create('2026-01-01')).toBeSuccessWithValue('2026-01-01');
    });

    it('should create a Date value for last day of year', () => {
      expect(DateOnly.create('2026-12-31')).toBeSuccessWithValue('2026-12-31');
    });

    it('should create a Date value for leap year date', () => {
      expect(DateOnly.create('2024-02-29')).toBeSuccessWithValue('2024-02-29');
    });

    it('should fail for invalid format without dashes', () => {
      const result = DateOnly.create('20260124');

      expect(result.isFailure).toBe(true);
      if (result.isFailure) {
        expect(result.errors).toContain(DateOnlyErrors.invalid);
      }
    });

    it('should fail for format with slashes', () => {
      expect(DateOnly.create('2026/01/24')).toBeFailureWithErrors([DateOnlyErrors.invalid]);
    });

    it('should fail for DD-MM-YYYY format', () => {
      expect(DateOnly.create('24-01-2026')).toBeFailureWithErrors([DateOnlyErrors.invalid]);
    });

    it('should fail for MM-DD-YYYY format', () => {
      expect(DateOnly.create('01-24-2026')).toBeFailureWithErrors([DateOnlyErrors.invalid]);
    });

    it('should fail for short year format', () => {
      expect(DateOnly.create('26-01-24')).toBeFailureWithErrors([DateOnlyErrors.invalid]);
    });

    it('should fail for single digit month', () => {
      expect(DateOnly.create('2026-1-24')).toBeFailureWithErrors([DateOnlyErrors.invalid]);
    });

    it('should fail for single digit day', () => {
      expect(DateOnly.create('2026-01-4')).toBeFailureWithErrors([DateOnlyErrors.invalid]);
    });

    it('should fail for empty string', () => {
      expect(DateOnly.create('')).toBeFailureWithErrors([DateOnlyErrors.invalid]);
    });

    it('should fail for invalid month value', () => {
      expect(DateOnly.create('2026-13-01')).toBeFailureWithErrors([DateOnlyErrors.invalid]);
    });

    it('should fail for invalid day value', () => {
      expect(DateOnly.create('2026-01-32')).toBeFailureWithErrors([DateOnlyErrors.invalid]);
    });

    it('should fail for non-date string', () => {
      expect(DateOnly.create('not-a-date')).toBeFailureWithErrors([DateOnlyErrors.invalid]);
    });

    it('should fail for null', () => {
      expect(DateOnly.create(null as any)).toBeFailureWithErrors([DateOnlyErrors.invalid]);
    });

    it('should fail for undefined', () => {
      expect(DateOnly.create(undefined as any)).toBeFailureWithErrors([DateOnlyErrors.invalid]);
    });

    it('should fail for number', () => {
      expect(DateOnly.create(20260124 as any)).toBeFailureWithErrors([DateOnlyErrors.invalid]);
    });

    it('should fail for date with time', () => {
      expect(DateOnly.create('2026-01-24T12:00:00')).toBeFailureWithErrors([DateOnlyErrors.invalid]);
    });

    it('should fail for date with timezone', () => {
      expect(DateOnly.create('2026-01-24Z')).toBeFailureWithErrors([DateOnlyErrors.invalid]);
    });

    it('should fail for extra characters', () => {
      expect(DateOnly.create('2026-01-24 ')).toBeFailureWithErrors([DateOnlyErrors.invalid]);
    });

    it('should fail for leading spaces', () => {
      expect(DateOnly.create(' 2026-01-24')).toBeFailureWithErrors([DateOnlyErrors.invalid]);
    });
  });

  describe('valid', () => {
    it('should create a DateOnly value for valid string', () => {
      expect(DateOnly.valid('2025-01-01')).toBe('2025-01-01');
    });
  });

  describe('Type safety', () => {
    it('should allow Date to be used as string', () => {
      const result = DateOnly.create('2026-01-24');

      if (result.isSuccess) {
        const date = result.value;
        const year = date.substring(0, 4);
        const month = date.substring(5, 7);
        const day = date.substring(8, 10);

        expect(year).toBe('2026');
        expect(month).toBe('01');
        expect(day).toBe('24');
      }
    });

    it('should allow Date to be compared with strict equality', () => {
      const result1 = DateOnly.create('2026-01-24');
      const result2 = DateOnly.create('2026-01-24');

      if (result1.isSuccess && result2.isSuccess) {
        expect(result1.value === result2.value).toBe(true);
      }
    });
  });

  describe('Edge cases', () => {
    it('should handle very old dates', () => {
      expect(DateOnly.create('1000-01-01')).toBeSuccessWithValue('1000-01-01');
    });

    it('should handle future dates', () => {
      expect(DateOnly.create('9999-12-31')).toBeSuccessWithValue('9999-12-31');
    });

    it('should handle year 2000', () => {
      expect(DateOnly.create('2000-01-01')).toBeSuccessWithValue('2000-01-01');
    });

    it('should fail for 3-digit year', () => {
      expect(DateOnly.create('999-01-01')).toBeFailureWithErrors([DateOnlyErrors.invalid]);
    });

    it('should fail for 5-digit year', () => {
      expect(DateOnly.create('10000-01-01')).toBeFailureWithErrors([DateOnlyErrors.invalid]);
    });
  });
});
