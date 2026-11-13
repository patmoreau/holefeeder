import { DateIntervalTypes } from '@holefeeder/shared/core';
import { aRecentDate, anAmount, anId, aString, aWord } from '@holefeeder/shared/testkit';
import { ModifyCashflowCommand } from '@/flows/core/flows/modify-cashflow/modify-cashflow-command';
import { TagList } from '@/flows/core/flows/tag-list';

const defaultModifyCashflowCommand = (): ModifyCashflowCommand => ({
  id: anId(),
  effectiveDate: aRecentDate(),
  amount: anAmount(),
  intervalType: DateIntervalTypes.monthly,
  frequency: 1,
  recurrence: 0,
  description: aString(),
  accountId: anId(),
  categoryId: anId(),
  tags: TagList.valid([aWord(), aWord()]),
});

export const aModifyCashflowCommand = (overrides?: Partial<ModifyCashflowCommand>): ModifyCashflowCommand => {
  return {
    ...defaultModifyCashflowCommand(),
    ...overrides,
  };
};
