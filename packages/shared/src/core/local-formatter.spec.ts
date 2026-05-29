import { DateOnly } from './date-only';
import { LocalFormatter } from './local-formatter';
import { withDate } from './with-date';

describe('LocalFormatter', () => {
  describe('currency', () => {
    it('should format amount with locale and currency code', () => {
      const formatted = LocalFormatter.currency(1234.56, 'en-US', 'USD');

      expect(formatted).toBe('$1,234.56');
    });

    it('should format amount with a different currency', () => {
      const formatted = LocalFormatter.currency(1234.56, 'en-US', 'EUR');

      expect(formatted).toBe('€1,234.56');
    });

    it('should fallback to CAD when currencyCode is empty', () => {
      const formatted = LocalFormatter.currency(1234.56, 'en-US', '');

      expect(formatted).toBe('CA$1,234.56');
    });

    it('should fallback to string format on error', () => {
      const originalNumberFormat = Intl.NumberFormat;
      Intl.NumberFormat = jest.fn().mockImplementation(() => {
        throw new Error('Invalid currency');
      }) as unknown as typeof Intl.NumberFormat;

      const formatted = LocalFormatter.currency(1234.56, 'en-US', 'INVALID');

      expect(formatted).toBe('1234.56 INVALID');

      Intl.NumberFormat = originalNumberFormat;
    });
  });

  describe('date', () => {
    const locale = 'en-US';
    const t = (key: string) => key;

    it('should format a DateOnly with Intl.DateTimeFormat', () => {
      const date = DateOnly.valid('2023-12-25');
      const anchor = DateOnly.valid('2025-12-25');

      const formatted = LocalFormatter.date(date, anchor, locale, t);

      expect(formatted).toMatch(/Dec/);
      expect(formatted).toMatch(/2023/);
    });

    it('should accept custom format options', () => {
      const date = DateOnly.valid('2023-12-25');
      const anchor = DateOnly.valid('2025-12-25');

      const formatted = LocalFormatter.date(date, anchor, locale, t, { dateStyle: 'short' });

      expect(formatted).toMatch(/12/);
      expect(formatted).toMatch(/2023|23/);
    });

    it('should return today label when date equals anchor', () => {
      const date = DateOnly.valid('2023-12-25');
      const anchor = DateOnly.valid('2023-12-25');

      const formatted = LocalFormatter.date(date, anchor, locale, t);

      expect(formatted).toMatch('common.today');
    });

    it('should return yesterday label when date is 1 day before anchor', () => {
      const date = DateOnly.valid('2023-12-24');
      const anchor = DateOnly.valid('2023-12-25');

      const formatted = LocalFormatter.date(date, anchor, locale, t);

      expect(formatted).toMatch('common.yesterday');
    });

    it('should return last7Days label when date is 2–7 days before anchor', () => {
      const date = DateOnly.valid('2023-12-24');
      const anchor = DateOnly.valid('2023-12-31');

      const formatted = LocalFormatter.date(date, anchor, locale, t);

      expect(formatted).toMatch('common.last7Days');
    });

    it('should return tomorrow label when date is 1 day after anchor', () => {
      const date = DateOnly.valid('2023-12-26');
      const anchor = DateOnly.valid('2023-12-25');

      const formatted = LocalFormatter.date(date, anchor, locale, t);

      expect(formatted).toMatch('common.tomorrow');
    });

    it('should return next7Days label when date is 2–7 days after anchor', () => {
      const date = DateOnly.valid('2023-12-31');
      const anchor = DateOnly.valid('2023-12-24');

      const formatted = LocalFormatter.date(date, anchor, locale, t);

      expect(formatted).toMatch('common.next7Days');
    });

    it('should fallback to toDateString on Intl error', () => {
      const date = DateOnly.valid('2023-12-25');
      const anchor = DateOnly.valid('2025-12-24');

      const originalDateTimeFormat = Intl.DateTimeFormat;
      Intl.DateTimeFormat = jest.fn().mockImplementation(() => {
        throw new Error('Invalid format');
      }) as unknown as typeof Intl.DateTimeFormat;

      const formatted = LocalFormatter.date(date, anchor, locale, t);

      expect(formatted).toBe(withDate(date).toDate().toDateString());

      Intl.DateTimeFormat = originalDateTimeFormat;
    });
  });

  describe('percentage', () => {
    it('should format percentage with 2 decimal places', () => {
      expect(LocalFormatter.percentage(12.3456)).toBe('12.35%');
    });

    it('should handle whole numbers', () => {
      expect(LocalFormatter.percentage(50)).toBe('50.00%');
    });

    it('should handle zero', () => {
      expect(LocalFormatter.percentage(0)).toBe('0.00%');
    });

    it('should handle negative values', () => {
      expect(LocalFormatter.percentage(-5.678)).toBe('-5.68%');
    });
  });
});
