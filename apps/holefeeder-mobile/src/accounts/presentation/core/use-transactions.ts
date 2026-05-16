import { useCallback } from 'react';
import { Transaction } from '@/flows/core/flows/transaction';
import { Id } from '@/shared/core/id';
import { type AsyncResult } from '@/shared/core/result';
import { usePagedWatch, type UsePagedWatchResult, WatchCountFn, WatchPageFn } from '@/shared/presentation/core/use-paged-watch';
import { useRepositories } from '@/shared/repositories/core/use-repositories';

export type UseTransactionsResult = UsePagedWatchResult<Transaction> & {
  transactions: AsyncResult<Transaction[]>;
};

export const useTransactions = (accountId: Id): UseTransactionsResult => {
  const { flowRepository } = useRepositories();

  const watchFn = useCallback<WatchPageFn<Transaction>>(
    (onData, limit, offset) =>
      flowRepository.watchTransactions(
        (result) => {
          if (result.isSuccess) onData(result.value);
        },
        accountId,
        limit,
        offset
      ),
    [flowRepository, accountId]
  );

  const watchCountFn = useCallback<WatchCountFn>(
    (onCount) =>
      flowRepository.watchTransactionCount((result) => {
        if (result.isSuccess) onCount(result.value);
      }, accountId),
    [flowRepository, accountId]
  );

  const paged = usePagedWatch(watchFn, watchCountFn);

  return { ...paged, transactions: paged.data };
};
