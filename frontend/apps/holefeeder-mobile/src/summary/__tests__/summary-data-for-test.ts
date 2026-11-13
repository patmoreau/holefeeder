import { aRecentDate, aCategoryType, anAmount } from '@holefeeder/shared/testkit';
import { SummaryData } from '@/summary/core/summary-data';

const defaultSummaryData = (): SummaryData => ({
  type: aCategoryType(),
  date: aRecentDate(),
  total: anAmount(),
});

export const aSummaryData = (overrides: Partial<SummaryData> = {}): SummaryData => ({ ...defaultSummaryData(), ...overrides });
