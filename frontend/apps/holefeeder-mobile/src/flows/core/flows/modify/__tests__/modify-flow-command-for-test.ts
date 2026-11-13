import { aRecentDate, anAmount, anId, aString, aWord } from '@holefeeder/shared/testkit';
import { ModifyFlowCommand } from '@/flows/core/flows/modify/modify-flow-command';
import { TagList } from '@/flows/core/flows/tag-list';

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
