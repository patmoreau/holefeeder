import { SummaryData } from '@/dashboard/core/summary-data';
import { aRecentDate } from '@/shared/__tests__/date-for-test';
import { aCategoryType } from '@/shared/__tests__/enum-for-test';
import { anAmount } from '@/shared/__tests__/number-for-test';

const defaultSummaryData = (): SummaryData => ({
  type: aCategoryType(),
  date: aRecentDate(),
  total: anAmount(),
});

export const aSummaryData = (overrides: Partial<SummaryData> = {}): SummaryData => ({ ...defaultSummaryData(), ...overrides });
