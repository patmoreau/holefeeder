import { useContext } from 'react';
import { BillsDueBadgeContext } from '@/settings/presentation/BillsDueBadgeProvider';
import { BillsDueBadgeState } from '@/settings/presentation/core/bills-due-badge-state';

export function useBillsDueBadge(): BillsDueBadgeState {
  const state = useContext(BillsDueBadgeContext);
  if (!state) {
    throw new Error('useBillsDueBadge must be used within a BillsDueBadgeProvider');
  }
  return state;
}
