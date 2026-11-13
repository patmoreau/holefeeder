import { aRecentDate, anAmount, anId } from '@holefeeder/shared/testkit';
import { PayFlowCommand } from '@/flows/core/flows/pay/pay-flow-command';

const defaultPayFlowCommand = (): PayFlowCommand => ({
  date: aRecentDate(),
  amount: anAmount(),
  cashflowId: anId(),
  cashflowDate: aRecentDate(),
  updateRecurringAmount: false,
});

export const aPayFlowCommand = (overrides?: Partial<PayFlowCommand>): PayFlowCommand => {
  return {
    ...defaultPayFlowCommand(),
    ...overrides,
  };
};
