import { useContext } from 'react';
import { CombinedInsightContext } from '@/statistics/presentation/CombinedInsightProvider';
import { CombinedInsightState } from '@/statistics/presentation/core/combined-insight-state';

export function useCombinedInsight(): CombinedInsightState {
  const state = useContext(CombinedInsightContext);
  if (!state) {
    throw new Error('useCombinedInsight must be used within a CombinedInsightProvider');
  }
  return state;
}
