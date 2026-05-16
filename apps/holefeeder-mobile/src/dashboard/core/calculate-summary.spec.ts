import { startOfMonth } from 'date-fns';
import { CategoryType, CategoryTypes } from '@/flows/core/categories/category-type';
import { DateIntervalTypes } from '@/shared/core/date-interval-type';
import { DateOnly } from '@/shared/core/date-only';
import { Money } from '@/shared/core/money';
import { today, withDate } from '@/shared/core/with-date';
import { calculateSummary } from './calculate-summary';
import { SummaryData } from './summary-data';

describe('calculateSummary', () => {
  const expense = CategoryType.valid(CategoryTypes.expense);
  const gain = CategoryType.valid(CategoryTypes.gain);

  it('should calculate summary correctly for weekly frequency', () => {
    const startDate = DateOnly.valid('2023-01-01');
    const data: SummaryData[] = [
      {
        type: expense,
        date: DateOnly.valid('2023-01-02'),
        total: (Money.create(100) as any).value,
      },
      {
        type: gain,
        date: DateOnly.valid('2023-01-03'),
        total: (Money.create(200) as any).value,
      },
    ];

    const result = calculateSummary(data, startDate, DateIntervalTypes.weekly, 1);

    expect(result.netFlow).toBe(100); // 200 - 100
    expect(result.currentExpenses).toBe(100);
    expect(result.currentGains).toBe(200);
  });

  it('should handle empty data', () => {
    const startDate = DateOnly.valid('2023-01-01');
    const result = calculateSummary([], startDate, DateIntervalTypes.weekly, 1);

    expect(result.netFlow).toBe(0);
    expect(result.currentExpenses).toBe(0);
    expect(result.currentGains).toBe(0);
    expect(result.averageExpenses).toBe(0);
  });

  it('should calculate summary correctly with complex monthly data', () => {
    const asOfDate = withDate(startOfMonth(today())).toDateOnly();
    const middleOfMonthDate = withDate(asOfDate).addDays(10).toDateOnly();
    const previousMonthDate = withDate(asOfDate).addMonths(-1).toDateOnly();
    const oldestMonthDate = withDate(asOfDate).addMonths(-2).toDateOnly();

    const effectiveDate = asOfDate;

    const data: SummaryData[] = [
      {
        type: expense,
        date: oldestMonthDate,
        total: Money.valid(100.1),
      },
      {
        type: expense,
        date: previousMonthDate,
        total: Money.valid(200.2),
      },
      {
        type: expense,
        date: asOfDate,
        total: Money.valid(800.8),
      },
      {
        type: expense,
        date: middleOfMonthDate,
        total: Money.valid(400.4),
      },
      {
        type: gain,
        date: oldestMonthDate,
        total: Money.valid(1000.1),
      },
      {
        type: gain,
        date: previousMonthDate,
        total: Money.valid(2000.2),
      },
      {
        type: gain,
        date: asOfDate,
        total: Money.valid(3000.3),
      },
    ];

    const result = calculateSummary(data, effectiveDate, DateIntervalTypes.monthly, 1);

    expect(result.currentExpenses).toBe(1201.2);
    expect(result.expenseVariation).toBe(700.7);
    expect(result.expenseVariationPercentage).toBe(140);
    expect(result.netFlow).toBe(1799.1);
    expect(result.currentGains).toBe(3000.3);
    expect(result.averageExpenses).toBe(500.5);
  });
});
