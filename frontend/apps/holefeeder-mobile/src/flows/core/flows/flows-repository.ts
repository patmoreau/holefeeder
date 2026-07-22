import { type AsyncResult, Id, Result } from '@holefeeder/shared/core';
import { AccountVariation } from '@/accounts/core/account-variation';
import { CashflowVariation } from '@/flows/core/flows/cashflow-variation';
import { CreateFlowCommand } from '@/flows/core/flows/create/create-flow-command';
import { ModifyFlowCommand } from '@/flows/core/flows/modify/modify-flow-command';
import { PayFlowCommand } from '@/flows/core/flows/pay/pay-flow-command';
import { Tag } from '@/flows/core/flows/tag';
import { Transaction } from '@/flows/core/flows/transaction';
import { TransferFlowCommand } from '@/flows/core/flows/transfer/transfer-flow-command';

export type FlowsRepository = {
  create(command: CreateFlowCommand): Promise<Result<Id>>;
  modify(command: ModifyFlowCommand): Promise<Result<Id>>;
  pay(command: PayFlowCommand): Promise<Result<Id>>;
  deactivateUpcoming(cashflowId: Id): Promise<Result<void>>;
  deleteTransaction(transactionId: Id): Promise<Result<void>>;
  transfer(command: TransferFlowCommand): Promise<Result<void>>;
  watchAccountVariation: (accountId: Id, onDataChange: (result: AsyncResult<AccountVariation | undefined>) => void) => () => void;
  watchCashflowVariations: (onDataChange: (result: AsyncResult<CashflowVariation[]>) => void) => () => void;
  watchTransaction: (transactionId: Id, onDataChange: (result: AsyncResult<Transaction>) => void) => () => void;
  watchTransactions: (
    onDataChange: (result: AsyncResult<Transaction[]>) => void,
    accountId?: Id,
    limit?: number,
    offset?: number
  ) => () => void;
  watchTransactionCount: (onDataChange: (result: AsyncResult<number>) => void, accountId?: Id) => () => void;
  watchTags: (onDataChange: (result: AsyncResult<Tag[]>) => void) => () => void;
};

export const FlowsRepositoryErrors = {
  createFlowCommandFailed: 'create-flow-command-failed',
  modifyFlowCommandFailed: 'modify-flow-command-failed',
  payFlowCommandFailed: 'pay-flow-command-failed',
  deleteTransactionCommandFailed: 'delete-transaction-command-failed',
  noTags: 'no-tags',
};
