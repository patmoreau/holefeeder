import { TransferFlowCommand } from '@/flows/core/flows/transfer/transfer-flow-command';
import { aRecentDate } from '@/shared/__tests__/date-for-test';
import { anAmount } from '@/shared/__tests__/number-for-test';
import { anId, aString } from '@/shared/__tests__/string-for-test';

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
