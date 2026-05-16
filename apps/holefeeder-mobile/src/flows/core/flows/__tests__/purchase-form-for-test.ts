import { aRecentDate } from '@/shared/__tests__/date-for-test';
import { anAmount } from '@/shared/__tests__/number-for-test';
import { anId, aString, aWord } from '@/shared/__tests__/string-for-test';
import { DateIntervalType, DateIntervalTypes } from '@/shared/core/date-interval-type';

type PurchaseForm = {
  date: string;
  amount: number;
  description: string;
  accountId: string;
  categoryId: string;
  tags: string[];
  cashflow?: {
    effectiveDate: string;
    intervalType: DateIntervalType;
    frequency: number;
    recurrence: number;
  };
};

const defaultPurchaseForm = (
  overrides?: Partial<{
    effectiveDate: string;
    intervalType: DateIntervalType;
    frequency: number;
    recurrence: number;
  }>
): PurchaseForm => ({
  date: aRecentDate(),
  amount: anAmount(),
  description: aString(),
  accountId: anId(),
  categoryId: anId(),
  tags: [aWord(), aWord()],
  cashflow: overrides
    ? { effectiveDate: aRecentDate(), intervalType: DateIntervalTypes.monthly, frequency: 1, recurrence: 1, ...overrides }
    : undefined,
});

type PurchaseFormOverrides = Omit<Partial<PurchaseForm>, 'cashflow'> & {
  cashflow?: Partial<NonNullable<PurchaseForm['cashflow']>>;
};

export const aPurchaseForm = (overrides?: PurchaseFormOverrides): PurchaseForm => {
  const { cashflow, ...rest } = overrides || {};
  return {
    ...defaultPurchaseForm(cashflow),
    ...rest,
  };
};
