import { type AsyncResult, DateInterval, Money, Result, today } from '@holefeeder/shared/core';
import { Settings } from '@/shared/core/settings';
import { calculateSummary, SummaryResult } from '../calculate-summary';
import { SummaryData } from '../summary-data';
import { SummaryRepository } from '../summary-repository';

export const WatchSummaryUseCase = (settings: Settings, repository: SummaryRepository) => {
  const watch = (onDataChange: (result: AsyncResult<ComputedSummary>) => void) =>
    repository.watch((result: AsyncResult<SummaryData[]>) => {
      if (result.isLoading || result.isFailure) {
        onDataChange(result);
        return;
      }

      const dateIntervalResult = DateInterval.createFrom(today(), 0, settings.effectiveDate, settings.intervalType, settings.frequency);
      if (!dateIntervalResult.isSuccess) {
        onDataChange(dateIntervalResult);
        return;
      }

      const summaryResult = calculateSummary(result.value, dateIntervalResult.value.start, settings.intervalType, settings.frequency);
      const computedSummary = computeDashboardData(summaryResult);

      onDataChange(Result.success(computedSummary));
    }, settings);

  return {
    watch: watch,
  };
};

export type ComputedSummary = {
  currentSpending: number;
  variation: {
    amount: number;
    percentage: number;
    isOver: boolean;
  };
  netFlow: {
    amount: number;
    isOver: boolean;
  };
  totalIncome: number;
  averageSpending: number;
  averageIncome: number;
  incomeVariation: {
    amount: number;
    percentage: number;
    isBelow: boolean;
  };
};

export const NO_SUMMARY: ComputedSummary = {
  currentSpending: 0,
  variation: {
    amount: 0,
    percentage: 0,
    isOver: false,
  },
  netFlow: {
    amount: 0,
    isOver: false,
  },
  totalIncome: 0,
  averageSpending: 0,
  averageIncome: 0,
  incomeVariation: {
    amount: 0,
    percentage: 0,
    isBelow: false,
  },
};

const computeDashboardData = (summary: SummaryResult): ComputedSummary => {
  const isOver = summary.expenseVariation > 0;

  return {
    currentSpending: summary.currentExpenses,
    variation: {
      amount: Math.abs(summary.expenseVariation),
      percentage: Math.abs(summary.expenseVariationPercentage),
      isOver,
    },
    netFlow: {
      amount: Math.abs(summary.netFlow),
      isOver: summary.netFlow > 0,
    },
    totalIncome: summary.currentGains,
    averageSpending: summary.averageExpenses,
    averageIncome: summary.averageGains,
    incomeVariation: computeIncomeVariation(summary.currentGains, summary.averageGains),
  };
};

const computeIncomeVariation = (currentGains: Money, averageGains: Money): ComputedSummary['incomeVariation'] => {
  const variationCents = Money.toCents(currentGains) - Money.toCents(averageGains);
  const averageCents = Money.toCents(averageGains);

  return {
    amount: Math.abs(variationCents) / 100,
    percentage: averageCents === 0 ? 0 : Math.round((Math.abs(variationCents) / averageCents) * 100 * 100) / 100,
    isBelow: variationCents < 0,
  };
};
