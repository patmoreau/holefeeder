import { ComputedSummary } from '@/summary/core/watch-summary/watch-summary-use-case';

const isShown = (summary: ComputedSummary): boolean => Math.round(summary.averageSpending * 100) > 0;

export const SpendingTrend = {
  isShown: isShown,
};
