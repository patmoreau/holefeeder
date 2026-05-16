import { ModifyFlowCommand } from '@/flows/core/flows/modify/modify-flow-command';
import { TagList } from '@/flows/core/flows/tag-list';
import { aRecentDate } from '@/shared/__tests__/date-for-test';
import { anAmount } from '@/shared/__tests__/number-for-test';
import { anId, aString, aWord } from '@/shared/__tests__/string-for-test';

const defaultModifyFlowCommand = (): ModifyFlowCommand => ({
  id: anId(),
  date: aRecentDate(),
  amount: anAmount(),
  description: aString(),
  accountId: anId(),
  categoryId: anId(),
  tags: TagList.valid([aWord(), aWord()]),
});

export const aModifyFlowCommand = (overrides?: Partial<ModifyFlowCommand>): ModifyFlowCommand => {
  return {
    ...defaultModifyFlowCommand(),
    ...overrides,
  };
};
