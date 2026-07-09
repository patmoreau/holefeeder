import { DateOnly, Id, Money, Result } from '@holefeeder/shared/core';

export type TransferFlowCommand = {
  date: DateOnly;
  amount: Money;
  description: string;
  sourceAccountId: Id;
  targetAccountId: Id;
};

const create = (transfer: Record<string, unknown>): Result<TransferFlowCommand> =>
  Result.combine<TransferFlowCommand>({
    date: DateOnly.create(transfer.date),
    amount: Money.create(transfer.amount),
    description: Result.success(transfer.description as string),
    sourceAccountId: Id.create(transfer.sourceAccountId),
    targetAccountId: Id.create(transfer.targetAccountId),
  });

export const TransferFlowCommand = {
  create: create,
};
