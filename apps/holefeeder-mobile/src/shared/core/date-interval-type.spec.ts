import { DateIntervalType, DateIntervalTypeErrors, DateIntervalTypes } from '@/shared/core/date-interval-type';
import { DateOnly } from './date-only';

describe('DateIntervalType', () => {
  describe('create', () => {
    it.each(Object.values(DateIntervalTypes))('accepts %s for DateIntervalType', (value) => {
      const result = DateIntervalType.create(value);
      expect(result).toBeSuccessWithValue(value);
    });

    it('rejects invalid DateIntervalType', () => {
      const result = DateIntervalType.create('invalid-type');
      expect(result).toBeFailureWithErrors([DateIntervalTypeErrors.invalid]);
    });

    it('rejects empty DateIntervalType', () => {
      const result = DateIntervalType.create('');
      expect(result).toBeFailureWithErrors([DateIntervalTypeErrors.invalid]);
    });

    it('rejects wrong type DateIntervalType', () => {
      const result = DateIntervalType.create(123);
      expect(result).toBeFailureWithErrors([DateIntervalTypeErrors.invalid]);
    });
  });

  it('valid returns the value directly', () => {
    const value = DateIntervalTypes.weekly;
    const result = DateIntervalType.valid(value);
    expect(result).toBe(value);
  });

  it('valid returns normalized value', () => {
    expect(DateIntervalType.valid(' Weekly ')).toBe(DateIntervalTypes.weekly);
    expect(DateIntervalType.valid(' OneTime ')).toBe(DateIntervalTypes.oneTime);
    expect(DateIntervalType.valid(' One_Time ')).toBe(DateIntervalTypes.oneTime);
  });

  it('create returns normalized value', () => {
    expect(DateIntervalType.create(' Weekly ')).toBeSuccessWithValue(DateIntervalTypes.weekly);
    expect(DateIntervalType.create(' OneTime ')).toBeSuccessWithValue(DateIntervalTypes.oneTime);
    expect(DateIntervalType.create(' One_Time ')).toBeSuccessWithValue(DateIntervalTypes.oneTime);
  });

  describe('addIteration', () => {
    it('add weeks to weekly', () => {
      const effectiveDate = DateOnly.valid('2023-01-01');
      const result = DateIntervalType.addIteration(effectiveDate, 1, DateIntervalTypes.weekly);
      expect(result).toBe('2023-01-08');
    });

    it('add months to monthly', () => {
      const effectiveDate = DateOnly.valid('2023-01-01');
      const result = DateIntervalType.addIteration(effectiveDate, 1, DateIntervalTypes.monthly);
      expect(result).toBe('2023-02-01');
    });

    it('add years to yearly', () => {
      const effectiveDate = DateOnly.valid('2023-01-01');
      const result = DateIntervalType.addIteration(effectiveDate, 1, DateIntervalTypes.yearly);
      expect(result).toBe('2024-01-01');
    });

    it('add oneTime to oneTime', () => {
      const effectiveDate = DateOnly.valid('2023-01-01');
      const result = DateIntervalType.addIteration(effectiveDate, 1, DateIntervalTypes.oneTime);
      expect(result).toBe('2023-01-01');
    });
  });

  describe('interval', () => {
    it.each([
      {
        effectiveDate: '2014-01-09',
        lookupDate: '2015-04-07',
        intervalType: DateIntervalTypes.weekly,
        frequency: 2,
        expected: { from: '2015-04-02', to: '2015-04-15' },
      },
      {
        effectiveDate: '2014-01-09',
        lookupDate: '2015-04-07',
        intervalType: DateIntervalTypes.weekly,
        frequency: 5,
        expected: { from: '2015-03-05', to: '2015-04-08' },
      },
      {
        effectiveDate: '2014-01-09',
        lookupDate: '2015-04-07',
        intervalType: DateIntervalTypes.monthly,
        frequency: 1,
        expected: { from: '2015-03-09', to: '2015-04-08' },
      },
      {
        effectiveDate: '2014-01-01',
        lookupDate: '2016-02-15',
        intervalType: DateIntervalTypes.monthly,
        frequency: 2,
        expected: { from: '2016-01-01', to: '2016-02-29' },
      },
      {
        effectiveDate: '2014-01-09',
        lookupDate: '2015-04-07',
        intervalType: DateIntervalTypes.yearly,
        frequency: 1,
        expected: { from: '2015-01-09', to: '2016-01-08' },
      },
      {
        effectiveDate: '2014-01-09',
        lookupDate: '2012-04-07',
        intervalType: DateIntervalTypes.yearly,
        frequency: 1,
        expected: { from: '2012-01-09', to: '2013-01-08' },
      },
      {
        effectiveDate: '2014-01-09',
        lookupDate: '2015-04-07',
        intervalType: DateIntervalTypes.daily,
        frequency: 3,
        expected: { from: '2015-04-07', to: '2015-04-09' },
      },
      {
        effectiveDate: '2014-01-09',
        lookupDate: '2015-04-07',
        intervalType: DateIntervalTypes.daily,
        frequency: 300,
        expected: { from: '2014-11-05', to: '2015-08-31' },
      },
    ])(
      'returns $expected.from - $expected.to for $intervalType starting $effectiveDate looking at $lookupDate',
      ({ effectiveDate, lookupDate, intervalType, frequency, expected }) => {
        const result = DateIntervalType.interval(DateOnly.valid(effectiveDate), DateOnly.valid(lookupDate), frequency, intervalType);
        expect(result).toEqual({ from: expected.from, to: expected.to });
      }
    );
  });

  describe('datesInRange', () => {
    const testCases: {
      intervalType: DateIntervalType;
      frequency: number;
      effectiveDate: DateOnly;
      fromDate: DateOnly;
      toDate: DateOnly;
      expected: DateOnly[];
    }[] = [
      {
        intervalType: DateIntervalTypes.oneTime,
        frequency: 1,
        effectiveDate: DateOnly.valid('2014-02-02'),
        fromDate: DateOnly.valid('2014-01-01'),
        toDate: DateOnly.valid('2014-03-01'),
        expected: [DateOnly.valid('2014-02-02')],
      },
      {
        intervalType: DateIntervalTypes.oneTime,
        frequency: 1,
        effectiveDate: DateOnly.valid('2014-02-02'),
        fromDate: DateOnly.valid('2014-02-02'),
        toDate: DateOnly.valid('2014-02-02'),
        expected: [DateOnly.valid('2014-02-02')],
      },
      {
        intervalType: DateIntervalTypes.oneTime,
        frequency: 1,
        effectiveDate: DateOnly.valid('2014-02-02'),
        fromDate: DateOnly.valid('2014-02-02'),
        toDate: DateOnly.valid('2014-03-01'),
        expected: [DateOnly.valid('2014-02-02')],
      },
      {
        intervalType: DateIntervalTypes.oneTime,
        frequency: 1,
        effectiveDate: DateOnly.valid('2014-02-02'),
        fromDate: DateOnly.valid('2014-01-01'),
        toDate: DateOnly.valid('2014-02-02'),
        expected: [DateOnly.valid('2014-02-02')],
      },
      {
        intervalType: DateIntervalTypes.oneTime,
        frequency: 1,
        effectiveDate: DateOnly.valid('2014-02-02'),
        fromDate: DateOnly.valid('2014-01-01'),
        toDate: DateOnly.valid('2014-01-01'),
        expected: [],
      },
      {
        intervalType: DateIntervalTypes.oneTime,
        frequency: 1,
        effectiveDate: DateOnly.valid('2014-02-02'),
        fromDate: DateOnly.valid('2014-03-01'),
        toDate: DateOnly.valid('2014-04-01'),
        expected: [],
      },
      {
        intervalType: DateIntervalTypes.weekly,
        frequency: 1,
        effectiveDate: DateOnly.valid('2014-02-02'),
        fromDate: DateOnly.valid('2014-01-01'),
        toDate: DateOnly.valid('2014-03-01'),
        expected: [DateOnly.valid('2014-02-02'), DateOnly.valid('2014-02-09'), DateOnly.valid('2014-02-16'), DateOnly.valid('2014-02-23')],
      },
      {
        intervalType: DateIntervalTypes.weekly,
        frequency: 1,
        effectiveDate: DateOnly.valid('2014-02-02'),
        fromDate: DateOnly.valid('2014-02-02'),
        toDate: DateOnly.valid('2014-02-02'),
        expected: [DateOnly.valid('2014-02-02')],
      },
      {
        intervalType: DateIntervalTypes.weekly,
        frequency: 1,
        effectiveDate: DateOnly.valid('2014-02-02'),
        fromDate: DateOnly.valid('2014-02-02'),
        toDate: DateOnly.valid('2014-03-01'),
        expected: [DateOnly.valid('2014-02-02'), DateOnly.valid('2014-02-09'), DateOnly.valid('2014-02-16'), DateOnly.valid('2014-02-23')],
      },
      {
        intervalType: DateIntervalTypes.weekly,
        frequency: 1,
        effectiveDate: DateOnly.valid('2014-02-02'),
        fromDate: DateOnly.valid('2014-01-01'),
        toDate: DateOnly.valid('2014-02-02'),
        expected: [DateOnly.valid('2014-02-02')],
      },
      {
        intervalType: DateIntervalTypes.weekly,
        frequency: 1,
        effectiveDate: DateOnly.valid('2014-02-02'),
        fromDate: DateOnly.valid('2014-01-01'),
        toDate: DateOnly.valid('2014-02-01'),
        expected: [],
      },
      {
        intervalType: DateIntervalTypes.monthly,
        frequency: 1,
        effectiveDate: DateOnly.valid('2014-02-02'),
        fromDate: DateOnly.valid('2014-01-01'),
        toDate: DateOnly.valid('2014-04-01'),
        expected: [DateOnly.valid('2014-02-02'), DateOnly.valid('2014-03-02')],
      },
      {
        intervalType: DateIntervalTypes.monthly,
        frequency: 1,
        effectiveDate: DateOnly.valid('2014-02-02'),
        fromDate: DateOnly.valid('2014-02-02'),
        toDate: DateOnly.valid('2014-02-02'),
        expected: [DateOnly.valid('2014-02-02')],
      },
      {
        intervalType: DateIntervalTypes.monthly,
        frequency: 1,
        effectiveDate: DateOnly.valid('2014-02-02'),
        fromDate: DateOnly.valid('2014-02-02'),
        toDate: DateOnly.valid('2014-04-01'),
        expected: [DateOnly.valid('2014-02-02'), DateOnly.valid('2014-03-02')],
      },
      {
        intervalType: DateIntervalTypes.monthly,
        frequency: 1,
        effectiveDate: DateOnly.valid('2014-02-02'),
        fromDate: DateOnly.valid('2014-01-01'),
        toDate: DateOnly.valid('2014-02-02'),
        expected: [DateOnly.valid('2014-02-02')],
      },
      {
        intervalType: DateIntervalTypes.monthly,
        frequency: 1,
        effectiveDate: DateOnly.valid('2014-02-02'),
        fromDate: DateOnly.valid('2014-01-01'),
        toDate: DateOnly.valid('2014-02-01'),
        expected: [],
      },
      {
        intervalType: DateIntervalTypes.yearly,
        frequency: 1,
        effectiveDate: DateOnly.valid('2014-02-02'),
        fromDate: DateOnly.valid('2014-01-01'),
        toDate: DateOnly.valid('2016-04-01'),
        expected: [DateOnly.valid('2014-02-02'), DateOnly.valid('2015-02-02'), DateOnly.valid('2016-02-02')],
      },
      {
        intervalType: DateIntervalTypes.yearly,
        frequency: 1,
        effectiveDate: DateOnly.valid('2014-02-02'),
        fromDate: DateOnly.valid('2014-02-02'),
        toDate: DateOnly.valid('2014-02-02'),
        expected: [DateOnly.valid('2014-02-02')],
      },
      {
        intervalType: DateIntervalTypes.yearly,
        frequency: 1,
        effectiveDate: DateOnly.valid('2014-02-02'),
        fromDate: DateOnly.valid('2014-02-02'),
        toDate: DateOnly.valid('2016-04-01'),
        expected: [DateOnly.valid('2014-02-02'), DateOnly.valid('2015-02-02'), DateOnly.valid('2016-02-02')],
      },
      {
        intervalType: DateIntervalTypes.yearly,
        frequency: 1,
        effectiveDate: DateOnly.valid('2014-02-02'),
        fromDate: DateOnly.valid('2013-01-01'),
        toDate: DateOnly.valid('2014-02-02'),
        expected: [DateOnly.valid('2014-02-02')],
      },
      {
        intervalType: DateIntervalTypes.yearly,
        frequency: 1,
        effectiveDate: DateOnly.valid('2014-02-02'),
        fromDate: DateOnly.valid('2013-01-01'),
        toDate: DateOnly.valid('2014-02-01'),
        expected: [],
      },
      {
        intervalType: DateIntervalTypes.daily,
        frequency: 1,
        effectiveDate: DateOnly.valid('2014-02-02'),
        fromDate: DateOnly.valid('2014-01-01'),
        toDate: DateOnly.valid('2014-02-10'),
        expected: [
          DateOnly.valid('2014-02-02'),
          DateOnly.valid('2014-02-03'),
          DateOnly.valid('2014-02-04'),
          DateOnly.valid('2014-02-05'),
          DateOnly.valid('2014-02-06'),
          DateOnly.valid('2014-02-07'),
          DateOnly.valid('2014-02-08'),
          DateOnly.valid('2014-02-09'),
          DateOnly.valid('2014-02-10'),
        ],
      },
      {
        intervalType: DateIntervalTypes.daily,
        frequency: 1,
        effectiveDate: DateOnly.valid('2014-02-02'),
        fromDate: DateOnly.valid('2014-02-02'),
        toDate: DateOnly.valid('2014-02-02'),
        expected: [DateOnly.valid('2014-02-02')],
      },
      {
        intervalType: DateIntervalTypes.daily,
        frequency: 1,
        effectiveDate: DateOnly.valid('2014-02-02'),
        fromDate: DateOnly.valid('2013-01-01'),
        toDate: DateOnly.valid('2014-02-01'),
        expected: [],
      },
    ];

    testCases.forEach(({ intervalType, frequency, effectiveDate, fromDate, toDate, expected }) => {
      it(`should return ${expected.length === 0 ? 'empty array' : `${expected.length} date(s)`} for ${intervalType} interval from ${fromDate} to ${toDate} with effective date ${effectiveDate}`, () => {
        const result = DateIntervalType.datesInRange(effectiveDate, fromDate, toDate, frequency, intervalType);
        expect(result).toEqual(expected);
      });
    });
  });
});
