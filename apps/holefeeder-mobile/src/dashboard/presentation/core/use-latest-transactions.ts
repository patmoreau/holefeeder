import { useCallback } from 'react';
import { Transaction } from '@/flows/core/flows/transaction';
import { type AsyncResult } from '@/shared/core/result';
import { usePagedWatch, type UsePagedWatchResult, WatchCountFn, WatchPageFn } from '@/shared/presentation/core/use-paged-watch';
import { useRepositories } from '@/shared/repositories/core/use-repositories';

export type UseLatestTransactionsResult = UsePagedWatchResult<Transaction> & {
  transactions: AsyncResult<Transaction[]>;
};

export const useLatestTransactions = (hardLimit = 3): UseLatestTransactionsResult => {
  const { flowRepository } = useRepositories();

  const watchFn = useCallback<WatchPageFn<Transaction>>(
    (onData, limit, offset) =>
      flowRepository.watchTransactions(
        (result) => {
          if (result.isSuccess) onData(result.value);
        },
        undefined,
        limit,
        offset
      ),
    [flowRepository]
  );

  const watchCountFn = useCallback<WatchCountFn>(
    (onCount) =>
      flowRepository.watchTransactionCount((result) => {
        if (result.isSuccess) onCount(result.value);
      }),
    [flowRepository]
  );

  const paged = usePagedWatch(watchFn, watchCountFn, { pageSize: hardLimit, maxPages: 1 });

  return { ...paged, transactions: paged.data };
};
