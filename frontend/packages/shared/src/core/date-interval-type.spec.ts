import { DateIntervalType, DateIntervalTypeErrors, DateIntervalTypes } from './date-interval-type';
import { DateOnly } from './date-only';
import { withDate } from './with-date';

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

    // Brute-force reference: the original one-period-at-a-time walk. Any optimized
    // implementation must produce identical results for every input.
    const referenceInterval = (
      effectiveDate: DateOnly,
      lookupDate: DateOnly,
      frequency: number,
      intervalType: (typeof DateIntervalTypes)[keyof typeof DateIntervalTypes]
    ): { from: DateOnly; to: DateOnly } => {
      if (intervalType === DateIntervalTypes.oneTime) {
        return { from: effectiveDate, to: effectiveDate };
      }
      const add = (n: number) => DateIntervalType.addIteration(effectiveDate, n, intervalType);
      const endOf = (start: DateOnly) =>
        withDate(DateIntervalType.addIteration(start, frequency, intervalType))
          .addDays(-1)
          .toDateOnly();
      const next = lookupDate > effectiveDate;
      let start = effectiveDate;
      let end = endOf(start);
      let iteration = 1;
      while (start > lookupDate || end < lookupDate) {
        start = add((next ? frequency : -frequency) * iteration);
        end = endOf(start);
        iteration++;
      }
      return { from: start, to: end };
    };

    const base = DateOnly.valid('2014-01-09');
    const offsets = [-4015, -1000, -370, -31, -1, 0, 1, 31, 400, 1500, 3000, 4600];
    const frequencies = [1, 2, 3, 5, 7, 12];

    it.each(Object.values(DateIntervalTypes))('matches the reference walk across large spans for %s', (intervalType) => {
      for (const effOffset of [0, 20, 355]) {
        const effectiveDate = withDate(base).addDays(effOffset).toDateOnly();
        for (const offset of offsets) {
          const lookupDate = withDate(effectiveDate).addDays(offset).toDateOnly();
          for (const frequency of frequencies) {
            const actual = DateIntervalType.interval(effectiveDate, lookupDate, frequency, intervalType);
            const expected = referenceInterval(effectiveDate, lookupDate, frequency, intervalType);
            expect({ intervalType, frequency, offset, ...actual }).toEqual({ intervalType, frequency, offset, ...expected });
          }
        }
      }
    });

    it('brackets the lookup date on the anchored grid for a 12-year span', () => {
      const effectiveDate = DateOnly.valid('2014-01-01');
      const lookupDate = DateOnly.valid('2026-07-11');
      const { from, to } = DateIntervalType.interval(effectiveDate, lookupDate, 1, DateIntervalTypes.monthly);
      expect(from <= lookupDate && lookupDate <= to).toBe(true);
      expect(to).toEqual(
        withDate(DateIntervalType.addIteration(from, 1, DateIntervalTypes.monthly))
          .addDays(-1)
          .toDateOnly()
      );
    });
  });

  describe('previousPeriods', () => {
    it.each([
      {
        description: 'weekly frequency 1',
        effectiveDate: '2023-01-01',
        intervalType: DateIntervalTypes.weekly,
        frequency: 1,
        beforeDate: '2023-01-22',
        expected: [
          { start: '2023-01-01', end: '2023-01-08' },
          { start: '2023-01-08', end: '2023-01-15' },
          { start: '2023-01-15', end: '2023-01-22' },
        ],
      },
      {
        description: 'weekly frequency 2',
        effectiveDate: '2023-01-01',
        intervalType: DateIntervalTypes.weekly,
        frequency: 2,
        beforeDate: '2023-01-29',
        expected: [
          { start: '2023-01-01', end: '2023-01-15' },
          { start: '2023-01-15', end: '2023-01-29' },
        ],
      },
      {
        description: 'monthly frequency 1',
        effectiveDate: '2023-01-01',
        intervalType: DateIntervalTypes.monthly,
        frequency: 1,
        beforeDate: '2023-04-01',
        expected: [
          { start: '2023-01-01', end: '2023-02-01' },
          { start: '2023-02-01', end: '2023-03-01' },
          { start: '2023-03-01', end: '2023-04-01' },
        ],
      },
      {
        description: 'monthly frequency 3',
        effectiveDate: '2023-01-01',
        intervalType: DateIntervalTypes.monthly,
        frequency: 3,
        beforeDate: '2023-10-01',
        expected: [
          { start: '2023-01-01', end: '2023-04-01' },
          { start: '2023-04-01', end: '2023-07-01' },
          { start: '2023-07-01', end: '2023-10-01' },
        ],
      },
      {
        description: 'month-boundary anchor on the 31st (short months clamp)',
        effectiveDate: '2023-01-31',
        intervalType: DateIntervalTypes.monthly,
        frequency: 1,
        beforeDate: '2023-04-30',
        expected: [
          { start: '2023-01-31', end: '2023-02-28' },
          { start: '2023-02-28', end: '2023-03-31' },
          { start: '2023-03-31', end: '2023-04-30' },
        ],
      },
      {
        description: 'zero previous periods (anchor === current period start)',
        effectiveDate: '2023-01-01',
        intervalType: DateIntervalTypes.weekly,
        frequency: 1,
        beforeDate: '2023-01-01',
        expected: [],
      },
      {
        description: 'oneTime never has previous periods',
        effectiveDate: '2023-01-01',
        intervalType: DateIntervalTypes.oneTime,
        frequency: 1,
        beforeDate: '2024-01-01',
        expected: [],
      },
    ])('returns complete past periods for $description', ({ effectiveDate, intervalType, frequency, beforeDate, expected }) => {
      const result = DateIntervalType.previousPeriods(DateOnly.valid(effectiveDate), intervalType, frequency, DateOnly.valid(beforeDate));
      expect(result).toEqual(expected);
    });

    it('excludes the incomplete period straddling the current period start', () => {
      // Current period starts 2023-01-18, which falls mid-way through the weekly grid.
      const result = DateIntervalType.previousPeriods(DateOnly.valid('2023-01-01'), DateIntervalTypes.weekly, 1, DateOnly.valid('2023-01-18'));
      expect(result).toEqual([
        { start: '2023-01-01', end: '2023-01-08' },
        { start: '2023-01-08', end: '2023-01-15' },
      ]);
    });

    it('produces contiguous periods where each end equals the next start', () => {
      const result = DateIntervalType.previousPeriods(DateOnly.valid('2023-01-01'), DateIntervalTypes.monthly, 1, DateOnly.valid('2023-06-01'));
      for (let i = 0; i < result.length - 1; i++) {
        expect(result[i].end).toBe(result[i + 1].start);
      }
    });
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
        intervalType: DateIntervalTypes.weekly,
        frequency: 1,
        effectiveDate: DateOnly.valid('2014-02-02'),
        fromDate: DateOnly.valid('2014-01-01'),
        toDate: DateOnly.valid('2014-03-01'),
        expected: [DateOnly.valid('2014-02-02'), DateOnly.valid('2014-02-09'), DateOnly.valid('2014-02-16'), DateOnly.valid('2014-02-23')],
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
        intervalType: DateIntervalTypes.yearly,
        frequency: 1,
        effectiveDate: DateOnly.valid('2014-02-02'),
        fromDate: DateOnly.valid('2014-01-01'),
        toDate: DateOnly.valid('2016-04-01'),
        expected: [DateOnly.valid('2014-02-02'), DateOnly.valid('2015-02-02'), DateOnly.valid('2016-02-02')],
      },
    ];

    testCases.forEach(({ intervalType, frequency, effectiveDate, fromDate, toDate, expected }) => {
      it(`should return ${expected.length === 0 ? 'empty array' : `${expected.length} date(s)`} for ${intervalType} interval from ${fromDate} to ${toDate}`, () => {
        const result = DateIntervalType.datesInRange(effectiveDate, fromDate, toDate, frequency, intervalType);
        expect(result).toEqual(expected);
      });
    });
  });
});
