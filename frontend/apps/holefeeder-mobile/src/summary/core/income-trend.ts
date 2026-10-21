import { ComputedSummary } from '@/summary/core/watch-summary/watch-summary-use-case';

const isShown = (summary: ComputedSummary): boolean => Math.round(summary.averageIncome * 100) > 0;

export const IncomeTrend = {
  isShown: isShown,
};
