import { type AsyncResult, DateInterval, Result, today } from '@holefeeder/shared/core';
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
  };
};
