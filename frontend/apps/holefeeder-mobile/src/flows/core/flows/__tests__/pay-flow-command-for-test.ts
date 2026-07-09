import { PayFlowCommand } from '@/flows/core/flows/pay/pay-flow-command';
import { aRecentDate } from '@/shared/__tests__/date-for-test';
import { anAmount } from '@/shared/__tests__/number-for-test';
import { anId } from '@/shared/__tests__/string-for-test';

const defaultPayFlowCommand = (): PayFlowCommand => ({
  date: aRecentDate(),
  amount: anAmount(),
  cashflowId: anId(),
  cashflowDate: aRecentDate(),
});

export const aPayFlowCommand = (overrides?: Partial<PayFlowCommand>): PayFlowCommand => {
  return {
    ...defaultPayFlowCommand(),
    ...overrides,
  };
};
