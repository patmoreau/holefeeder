import { useContext } from 'react';
import { InsightsRangeState } from '@/statistics/presentation/core/insights-range-state';
import { InsightsRangeContext } from '@/statistics/presentation/InsightsRangeProvider';

export function useInsightsRange(): InsightsRangeState {
  const state = useContext(InsightsRangeContext);
  if (!state) {
    throw new Error('useInsightsRange must be used within an InsightsRangeProvider');
  }
  return state;
}
