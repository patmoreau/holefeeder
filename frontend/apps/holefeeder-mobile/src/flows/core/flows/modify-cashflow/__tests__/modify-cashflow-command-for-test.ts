import { DateIntervalTypes } from '@holefeeder/shared/core';
import { ModifyCashflowCommand } from '@/flows/core/flows/modify-cashflow/modify-cashflow-command';
import { TagList } from '@/flows/core/flows/tag-list';
import { aRecentDate } from '@/shared/__tests__/date-for-test';
import { anAmount } from '@/shared/__tests__/number-for-test';
import { anId, aString, aWord } from '@/shared/__tests__/string-for-test';

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
