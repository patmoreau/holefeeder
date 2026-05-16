import { type AsyncResult, Id, Result } from '@holefeeder/core';
import { AccountVariation } from '@/accounts/core/account-variation';
import { CashflowVariation } from '@/flows/core/flows/cashflow-variation';
import { CreateFlowCommand } from '@/flows/core/flows/create/create-flow-command';
import { FlowsRepository } from '@/flows/core/flows/flows-repository';
import { ModifyFlowCommand } from '@/flows/core/flows/modify/modify-flow-command';
import { PayFlowCommand } from '@/flows/core/flows/pay/pay-flow-command';
import { Tag } from '@/flows/core/flows/tag';
import { Transaction } from '@/flows/core/flows/transaction';
import { TransferFlowCommand } from '@/flows/core/flows/transfer/transfer-flow-command';
import { anId } from '@/shared/__tests__/string-for-test';

export type FlowsRepositoryInMemory = FlowsRepository & {
  addAccountVariations: (...items: AccountVariation[]) => void;
  addCashflowVariations: (...items: CashflowVariation[]) => void;
  addTags: (...items: Tag[]) => void;
  addTransactions: (...items: Transaction[]) => void;
  isLoading: () => void;
  isFailing: (errors: string[]) => void;
};

export const FlowsRepositoryInMemory = (): FlowsRepositoryInMemory => {
  const accountVariationsInMemory: AccountVariation[] = [];
  const cashflowVariationsInMemory: CashflowVariation[] = [];
  const tagsInMemory: Tag[] = [];
  const transactionsInMemory: Transaction[] = [];
  let loadingInMemory = false;
  let errorsInMemory: string[] = [];

  type Listener<T> = (result: AsyncResult<T>) => void;
  type AccountVariationListener = { accountId: Id; listener: Listener<AccountVariation | undefined> };
  type TransactionListener = { transactionId: Id; listener: Listener<Transaction> };
  type TransactionsListener = { listener: Listener<Transaction[]>; accountId?: Id; limit?: number; offset?: number };
  type LatestTransactionsListener = { listener: Listener<Transaction[]>; limit?: number };
  type TransactionCountListener = { listener: Listener<number>; accountId?: Id };

  const accountVariationListeners: AccountVariationListener[] = [];
  const cashflowVariationsListeners: Listener<CashflowVariation[]>[] = [];
  const tagsListeners: Listener<Tag[]>[] = [];
  const latestTransactionsListeners: LatestTransactionsListener[] = [];
  const transactionListeners: TransactionListener[] = [];
  const transactionsListeners: TransactionsListener[] = [];
  const transactionCountListeners: TransactionCountListener[] = [];

  const emit = <T>(listeners: Listener<T>[], result: AsyncResult<T>) => listeners.forEach((l) => l(result));

  const currentResult = <T>(value: T): AsyncResult<T> => {
    if (loadingInMemory) return Result.loading();
    if (errorsInMemory.length > 0) return Result.failure(errorsInMemory);
    return Result.success(value);
  };

  const notifyAll = () => {
    accountVariationListeners.forEach(({ accountId, listener }) => {
      const found = accountVariationsInMemory.find((v) => v.accountId === accountId);
      listener(currentResult(found));
    });
    emit(cashflowVariationsListeners, currentResult(cashflowVariationsInMemory));
    emit(tagsListeners, currentResult(tagsInMemory));
    latestTransactionsListeners.forEach(({ listener, limit }) => listener(currentResult(transactionsInMemory.slice(0, limit))));
    transactionsListeners.forEach(({ listener, accountId, limit, offset }) => {
      const filtered = transactionsInMemory
        .filter((t) => (accountId ? t.accountId === accountId : true))
        .slice(offset ?? 0, limit !== undefined ? (offset ?? 0) + limit : undefined);
      listener(currentResult(filtered));
    });
    transactionCountListeners.forEach(({ listener, accountId }) => {
      const length = transactionsInMemory.filter((t) => (accountId ? t.accountId === accountId : true)).length;
      listener(currentResult(length));
    });
  };

  const subscribe = <T>(listeners: Listener<T>[], listener: Listener<T>, initialResult: AsyncResult<T>) => {
    listeners.push(listener);
    listener(initialResult);
    return () => listeners.splice(listeners.indexOf(listener), 1);
  };

  const create = (_purchase: CreateFlowCommand): Promise<Result<Id>> => {
    return Promise.resolve(Result.success(anId()));
  };

  const modify = (_purchase: ModifyFlowCommand): Promise<Result<Id>> => {
    return Promise.resolve(Result.success(anId()));
  };

  const pay = (_command: PayFlowCommand): Promise<Result<Id>> => {
    return Promise.resolve(Result.success(anId()));
  };

  const deactivateUpcoming = (_cashflowId: Id): Promise<Result<void>> => {
    return Promise.resolve(Result.success());
  };

  const transfer = (_command: TransferFlowCommand): Promise<Result<void>> => {
    return Promise.resolve(Result.success());
  };

  const watchAccountVariation = (accountId: Id, onDataChange: Listener<AccountVariation | undefined>) => {
    const entry: AccountVariationListener = { accountId, listener: onDataChange };
    accountVariationListeners.push(entry);
    const found = accountVariationsInMemory.find((v) => v.accountId === accountId);
    onDataChange(currentResult(found));
    return () => accountVariationListeners.splice(accountVariationListeners.indexOf(entry), 1);
  };

  const watchCashflowVariations = (onDataChange: Listener<CashflowVariation[]>) =>
    subscribe(cashflowVariationsListeners, onDataChange, currentResult(cashflowVariationsInMemory));

  const watchTags = (onDataChange: Listener<Tag[]>) => subscribe(tagsListeners, onDataChange, currentResult(tagsInMemory));

  const watchTransaction = (transactionId: Id, onDataChange: Listener<Transaction>) => {
    const entry: TransactionListener = { transactionId, listener: onDataChange };
    transactionListeners.push(entry);
    const filtered = transactionsInMemory.filter((t) => t.id === transactionId).slice(0, 1)[0];
    onDataChange(currentResult(filtered));
    return () => transactionListeners.splice(transactionListeners.indexOf(entry), 1);
  };

  const watchTransactions = (onDataChange: Listener<Transaction[]>, accountId?: Id, limit?: number, offset?: number) => {
    const entry: TransactionsListener = { listener: onDataChange, accountId, limit, offset };
    transactionsListeners.push(entry);
    const filtered = transactionsInMemory
      .filter((t) => (accountId ? t.accountId === accountId : true))
      .slice(offset ?? 0, limit !== undefined ? (offset ?? 0) + limit : undefined);
    onDataChange(currentResult(filtered));
    return () => transactionsListeners.splice(transactionsListeners.indexOf(entry), 1);
  };

  const watchTransactionCount = (onDataChange: Listener<number>, accountId?: Id) => {
    const entry: TransactionCountListener = { listener: onDataChange, accountId };
    transactionCountListeners.push(entry);
    const length = transactionsInMemory.filter((t) => (accountId ? t.accountId === accountId : true)).length;
    onDataChange(currentResult(length));
    return () => transactionCountListeners.splice(transactionCountListeners.indexOf(entry), 1);
  };

  const addAccountVariations = (...items: AccountVariation[]) => {
    accountVariationsInMemory.push(...items);
    accountVariationListeners.forEach(({ accountId, listener }) => {
      const found = accountVariationsInMemory.find((v) => v.accountId === accountId);
      listener(currentResult(found));
    });
  };

  const addCashflowVariations = (...items: CashflowVariation[]) => {
    cashflowVariationsInMemory.push(...items);
    emit(cashflowVariationsListeners, currentResult(cashflowVariationsInMemory));
  };

  const addTags = (...items: Tag[]) => {
    tagsInMemory.push(...items);
    emit(tagsListeners, currentResult(tagsInMemory));
  };

  const addTransactions = (...items: Transaction[]) => {
    transactionsInMemory.push(...items);
    latestTransactionsListeners.forEach(({ listener, limit }) => listener(currentResult(transactionsInMemory.slice(0, limit))));
    transactionsListeners.forEach(({ listener, accountId, limit, offset }) => {
      const filtered = transactionsInMemory
        .filter((t) => (accountId ? t.accountId === accountId : true))
        .slice(offset ?? 0, limit !== undefined ? (offset ?? 0) + limit : undefined);
      listener(currentResult(filtered));
    });
    transactionCountListeners.forEach(({ listener, accountId }) => {
      const length = transactionsInMemory.filter((t) => (accountId ? t.accountId === accountId : true)).length;
      listener(currentResult(length));
    });
  };

  const isLoading = () => {
    loadingInMemory = true;
    notifyAll();
  };

  const isFailing = (errors: string[]) => {
    errorsInMemory = errors;
    notifyAll();
  };

  return {
    create: create,
    modify: modify,
    pay: pay,
    deactivateUpcoming: deactivateUpcoming,
    transfer: transfer,
    watchAccountVariation: watchAccountVariation,
    watchCashflowVariations: watchCashflowVariations,
    watchTags: watchTags,
    watchTransaction: watchTransaction,
    watchTransactions: watchTransactions,
    watchTransactionCount: watchTransactionCount,
    addAccountVariations: addAccountVariations,
    addCashflowVariations: addCashflowVariations,
    addTags: addTags,
    addTransactions: addTransactions,
    isLoading: isLoading,
    isFailing: isFailing,
  };
};
