import { SummaryResult } from '@/dashboard/core/calculate-summary';
import { anAmount } from '@/shared/__tests__/number-for-test';

const defaultSummary = (): SummaryResult => ({
  currentExpenses: anAmount(),
  expenseVariation: anAmount(),
  expenseVariationPercentage: anAmount(),
  netFlow: anAmount(),
  currentGains: anAmount(),
  averageExpenses: anAmount(),
});

export const aSummary = (overrides: Partial<SummaryResult> = {}): SummaryResult => ({ ...defaultSummary(), ...overrides });
