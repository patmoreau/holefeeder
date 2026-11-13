import { DateIntervalType, DateIntervalTypes, DateOnly } from '@holefeeder/shared/core';
import { aRecentDate, anAmount, anId, aString, aWord } from '@holefeeder/shared/testkit';
import { CreateFlowCommand } from '@/flows/core/flows/create/create-flow-command';
import { TagList } from '@/flows/core/flows/tag-list';

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
