import { aPastDate, aCategoryType, aDateIntervalType, aCount, anAmount, anId, aString } from '@holefeeder/shared/testkit';
import { aTagList } from '@/flows/core/flows/__tests__/tag-list-for-test';
import { CashflowVariation } from '@/flows/core/flows/cashflow-variation';

const defaultCashflowVariation = (): CashflowVariation => ({
  id: anId(),
  accountId: anId(),
  lastPaidDate: aPastDate(),
  lastCashflowDate: aPastDate(),
  amount: anAmount(),
  description: aString(),
  effectiveDate: aPastDate(),
  frequency: aCount(),
  intervalType: aDateIntervalType(),
  categoryType: aCategoryType(),
  categoryName: aString(),
  categoryColor: '#3B82F6',
  tags: aTagList(),
});

export const aCashflowVariation = (overrides?: Partial<CashflowVariation>): CashflowVariation => ({
  ...defaultCashflowVariation(),
  ...overrides,
});
