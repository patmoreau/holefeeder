import { useContext } from 'react';
import { BillsDueBadgeState } from '@/shared/bills-due-badge/core/bills-due-badge-state';
import { BillsDueBadgeContext } from '@/shared/bills-due-badge/presentation/BillsDueBadgeProvider';

export function useBillsDueBadge(): BillsDueBadgeState {
  const state = useContext(BillsDueBadgeContext);
  if (!state) {
    throw new Error('useBillsDueBadge must be used within a BillsDueBadgeProvider');
  }
  return state;
}
