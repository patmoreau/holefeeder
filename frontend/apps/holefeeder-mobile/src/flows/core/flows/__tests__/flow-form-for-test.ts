import { aRecentDate } from '@/shared/__tests__/date-for-test';
import { anAmount } from '@/shared/__tests__/number-for-test';
import { anId, aString, aWord } from '@/shared/__tests__/string-for-test';

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
