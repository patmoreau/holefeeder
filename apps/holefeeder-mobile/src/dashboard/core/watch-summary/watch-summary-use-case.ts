import { Settings } from '@/settings/core/settings';
import { type AsyncResult, Result } from '@/shared/core/result';
import { today } from '@/shared/core/with-date';
import { DateInterval } from '../../../shared/core/date-interval';
import { calculateSummary, SummaryResult } from '../calculate-summary';
import { DashboardRepository } from '../dashboard-repository';
import { SummaryData } from '../summary-data';

export const WatchSummaryUseCase = (settings: Settings, repository: DashboardRepository) => {
  const watch = (onDataChange: (result: AsyncResult<DashboardComputedSummary>) => void) =>
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

export type DashboardComputedSummary = {
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

export const NO_SUMMARY: DashboardComputedSummary = {
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

const computeDashboardData = (summary: SummaryResult): DashboardComputedSummary => {
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
