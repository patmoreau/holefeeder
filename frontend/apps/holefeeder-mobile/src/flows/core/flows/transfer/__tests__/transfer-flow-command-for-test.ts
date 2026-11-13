import { aRecentDate, anAmount, anId, aString } from '@holefeeder/shared/testkit';
import { TransferFlowCommand } from '@/flows/core/flows/transfer/transfer-flow-command';

const defaultTransferFlowCommand = (): TransferFlowCommand => ({
  date: aRecentDate(),
  amount: anAmount(),
  description: aString(),
  sourceAccountId: anId(),
  targetAccountId: anId(),
});

export const aTransferFlowCommand = (overrides?: Partial<TransferFlowCommand>): TransferFlowCommand => ({
  ...defaultTransferFlowCommand(),
  ...overrides,
});
