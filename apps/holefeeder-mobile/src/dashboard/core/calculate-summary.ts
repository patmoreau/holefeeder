import { CategoryType, CategoryTypes } from '@/flows/core/categories/category-type';
import { DateIntervalType } from '@/shared/core/date-interval-type';
import { DateOnly } from '@/shared/core/date-only';
import { Money } from '@/shared/core/money';
import { SummaryData } from './summary-data';

export type SummaryResult = {
  currentExpenses: Money;
  expenseVariation: number;
  expenseVariationPercentage: number;
  netFlow: number;
  currentGains: Money;
  averageExpenses: Money;
};

export const calculateSummary = (
  data: SummaryData[],
  startDate: DateOnly,
  intervalType: DateIntervalType,
  frequency: number
): SummaryResult => {
  const groupedByTypeAndPeriod = groupByTypeAndPeriod(data, startDate, intervalType, frequency);

  const gains = filterByType(groupedByTypeAndPeriod, CategoryTypes.gain);
  const expenses = filterByType(groupedByTypeAndPeriod, CategoryTypes.expense);
  const currentGains = getPeriodTotal(gains, startDate);
  const currentExpenses = getPeriodTotal(expenses, startDate);
  const netFlow = calculateNetFlow(currentGains, currentExpenses);

  const averageExpenses = calculateAverage(expenses);
  const expenseVariation = calculateExpenseVariation(averageExpenses, currentExpenses);
  const variationPercentage = calculateExpenseVariationPercentage(averageExpenses, expenseVariation);

  return {
    currentExpenses,
    expenseVariation,
    expenseVariationPercentage: variationPercentage,
    netFlow,
    currentGains,
    averageExpenses,
  };
};

const calculateNetFlow = (currentGains: Money, currentExpenses: Money): number => {
  return Math.round((Money.toCents(currentGains) / 100 - Money.toCents(currentExpenses) / 100) * 100) / 100;
};

const calculateExpenseVariation = (averageExpenses: Money, currentExpenses: Money): number => {
  return Money.toCents(currentExpenses) / 100 - Money.toCents(averageExpenses) / 100;
};

const calculateExpenseVariationPercentage = (averageExpenses: Money, expenseVariation: number): number => {
  if (averageExpenses === Money.ZERO) {
    return 0;
  }
  const avg = Money.toCents(averageExpenses) / 100;
  return Math.round((expenseVariation / avg) * 100 * 100) / 100;
};

type GroupedData = {
  type: CategoryType;
  period: { from: DateOnly; to: DateOnly };
  total: Money;
};

const groupByTypeAndPeriod = (data: SummaryData[], startDate: DateOnly, intervalType: DateIntervalType, frequency: number): GroupedData[] => {
  const grouped = new Map<string, GroupedData>();

  for (const item of data) {
    const period = DateIntervalType.interval(startDate, item.date, frequency, intervalType);
    const key = `${item.type}-${period.from}-${period.to}`;

    if (!grouped.has(key)) {
      grouped.set(key, {
        type: item.type,
        period,
        total: Money.ZERO,
      });
    }

    const current = grouped.get(key)!;
    const newTotal = (Money.toCents(current.total) + Money.toCents(item.total)) / 100;
    const moneyResult = Money.create(newTotal);
    if (moneyResult.isSuccess) {
      current.total = moneyResult.value;
    }
  }

  return Array.from(grouped.values());
};

const filterByType = (data: GroupedData[], categoryType: CategoryType): Map<string, Money> => {
  const result = new Map<string, Money>();
  for (const item of data) {
    if (item.type === categoryType) {
      // Map key needs to handle the object structure.
      // In C# it was a dictionary of (From, To) -> Money.
      // Here we can use a string key representation of the period.
      const key = `${item.period.from}|${item.period.to}`;

      // Note: The C# code does ToDictionary, implying unique keys (Period).
      // GroupByTypeAndPeriod already groups by Period, so uniqueness is guaranteed.
      result.set(key, item.total);
    }
  }
  return result;
};

const getPeriodTotal = (periodData: Map<string, Money>, asOf: DateOnly): Money => {
  // Logic: periodData.Where(x => x.Key.From == asOf).Select(x => x.Value).FirstOrDefault(Money.Zero);

  for (const [key, value] of periodData.entries()) {
    const [from] = key.split('|');
    if (from === asOf) {
      return value;
    }
  }

  return Money.ZERO;
};

const calculateAverage = (periodData: Map<string, Money>): Money => {
  if (periodData.size === 0) {
    return Money.ZERO;
  }

  let totalCents = 0;
  for (const value of periodData.values()) {
    totalCents += Money.toCents(value);
  }

  const average = totalCents / periodData.size / 100;
  const moneyResult = Money.create(average);
  return moneyResult.isSuccess ? moneyResult.value : Money.ZERO;
};
