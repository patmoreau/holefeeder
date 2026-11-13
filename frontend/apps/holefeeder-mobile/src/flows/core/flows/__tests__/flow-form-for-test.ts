import { aRecentDate, anAmount, anId, aString, aWord } from '@holefeeder/shared/testkit';

type FlowForm = {
  id: string;
  date: string;
  amount: number;
  description: string;
  accountId: string;
  categoryId: string;
  tags: string[];
};

const defaultFlowForm = (): FlowForm => ({
  id: anId(),
  date: aRecentDate(),
  amount: anAmount(),
  description: aString(),
  accountId: anId(),
  categoryId: anId(),
  tags: [aWord(), aWord()],
});

export const aFlowForm = (overrides?: Partial<FlowForm>): FlowForm => {
  return {
    ...defaultFlowForm(),
    ...overrides,
  };
};
