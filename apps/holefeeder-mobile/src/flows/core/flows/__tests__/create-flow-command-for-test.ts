import { CreateFlowCommand } from '@/flows/core/flows/create/create-flow-command';
import { TagList } from '@/flows/core/flows/tag-list';
import { aRecentDate } from '@/shared/__tests__/date-for-test';
import { anAmount } from '@/shared/__tests__/number-for-test';
import { anId, aString, aWord } from '@/shared/__tests__/string-for-test';
import { DateIntervalType, DateIntervalTypes } from '@/shared/core/date-interval-type';
import { DateOnly } from '@/shared/core/date-only';

const defaultCreateFlowCommand = (
  overrides?: Partial<{
    effectiveDate: DateOnly;
    intervalType: DateIntervalType;
    frequency: number;
    recurrence: number;
  }>
): CreateFlowCommand => ({
  date: aRecentDate(),
  amount: anAmount(),
  description: aString(),
  accountId: anId(),
  categoryId: anId(),
  tags: TagList.valid([aWord(), aWord()]),
  cashflow: overrides
    ? {
        effectiveDate: aRecentDate(),
        intervalType: DateIntervalTypes.monthly,
        frequency: 1,
        recurrence: 1,
        ...overrides,
      }
    : undefined,
});

type CreateFlowCommandOverrides = Omit<Partial<CreateFlowCommand>, 'cashflow'> & {
  cashflow?: Partial<NonNullable<CreateFlowCommand['cashflow']>>;
};

export const aCreateFlowCommand = (overrides?: CreateFlowCommandOverrides): CreateFlowCommand => {
  const { cashflow, ...rest } = overrides || {};
  return {
    ...defaultCreateFlowCommand(cashflow),
    ...rest,
  };
};
