import React from 'react';
import { CategorySpendingList } from './CategorySpendingList';
import { CombinedInsightToggle } from './CombinedInsightToggle';
import { CombinedSpendingList } from './CombinedSpendingList';
import { useCombinedInsight } from './core/use-combined-insight';
import { TagSpendingList } from './TagSpendingList';

export const SpendingBreakdown = () => {
  const { combined } = useCombinedInsight();

  return (
    <>
      <CombinedInsightToggle />
      {combined ? (
        <CombinedSpendingList />
      ) : (
        <>
          <CategorySpendingList />
          <TagSpendingList />
        </>
      )}
    </>
  );
};
