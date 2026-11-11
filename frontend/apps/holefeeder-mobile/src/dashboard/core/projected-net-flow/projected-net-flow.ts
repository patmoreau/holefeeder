import { Money } from '@holefeeder/shared/core';
import { UpcomingFlow } from '@/flows/core/flows/upcoming-flow';
import { CategoryType } from '@/shared/core/category-type';
import { ComputedSummary } from '@/summary/core/watch-summary/watch-summary-use-case';

export const projectedNetFlow = (summary: ComputedSummary, upcomingFlows: UpcomingFlow[]): number => {
  const netFlow = summary.netFlow.isOver ? summary.netFlow.amount : -summary.netFlow.amount;
  const upcomingVariation = upcomingFlows.reduce(
    (acc, flow) => acc + (Money.toCents(flow.amount) / 100) * CategoryType.multiplier[flow.categoryType],
    0
  );
  return netFlow + upcomingVariation;
};
