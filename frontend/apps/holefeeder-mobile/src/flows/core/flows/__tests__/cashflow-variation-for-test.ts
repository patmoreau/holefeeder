import { aTagList } from '@/flows/core/flows/__tests__/tag-list-for-test';
import { CashflowVariation } from '@/flows/core/flows/cashflow-variation';
import { aPastDate } from '@/shared/__tests__/date-for-test';
import { aCategoryType, aDateIntervalType } from '@/shared/__tests__/enum-for-test';
import { aCount, anAmount } from '@/shared/__tests__/number-for-test';
import { anId, aString } from '@/shared/__tests__/string-for-test';

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
  tags: aTagList(),
});

export const aCashflowVariation = (overrides?: Partial<CashflowVariation>): CashflowVariation => ({
  ...defaultCashflowVariation(),
  ...overrides,
});
