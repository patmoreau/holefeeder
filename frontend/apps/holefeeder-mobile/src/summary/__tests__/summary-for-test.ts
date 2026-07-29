import { anAmount } from '@/shared/__tests__/number-for-test';
import { SummaryResult } from '@/summary/core/calculate-summary';

const defaultSummary = (): SummaryResult => ({
  currentExpenses: anAmount(),
  expenseVariation: anAmount(),
  expenseVariationPercentage: anAmount(),
  netFlow: anAmount(),
  currentGains: anAmount(),
  averageExpenses: anAmount(),
});

export const aSummary = (overrides: Partial<SummaryResult> = {}): SummaryResult => ({ ...defaultSummary(), ...overrides });
