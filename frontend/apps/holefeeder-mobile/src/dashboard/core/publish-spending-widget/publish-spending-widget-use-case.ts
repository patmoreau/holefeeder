import { Result } from '@holefeeder/shared/core';
import { projectedNetFlow } from '@/dashboard/core/projected-net-flow/projected-net-flow';
import { UpcomingFlow } from '@/flows/core/flows/upcoming-flow';
import { SpendingTrend } from '@/summary/core/spending-trend';
import { ComputedSummary } from '@/summary/core/watch-summary/watch-summary-use-case';
import { SpendingSnapshot } from './spending-snapshot';
import { SpendingWidgetPublisher } from './spending-widget-publisher';

export const PublishSpendingWidgetUseCase = (publisher: SpendingWidgetPublisher) => {
  const execute = async (summary: ComputedSummary, upcomingFlows: UpcomingFlow[]): Promise<Result<void>> => {
    const snapshot: SpendingSnapshot = {
      spending: summary.currentSpending,
      projected: projectedNetFlow(summary, upcomingFlows),
    };
    if (SpendingTrend.isShown(summary)) snapshot.trend = { amount: summary.variation.amount, isOver: summary.variation.isOver };
    return await publisher.publish(snapshot);
  };

  const clear = async (): Promise<Result<void>> => await publisher.clear();

  return {
    execute: execute,
    clear: clear,
  };
};
