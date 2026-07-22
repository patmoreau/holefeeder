import { type AsyncResult, Id, Result } from '@holefeeder/shared/core';
import { useCallback, useEffect, useMemo, useState } from 'react';
import { Transaction } from '@/flows/core/flows/transaction';
import { useRepositories } from '@/shared/repositories/core/use-repositories';

export type UseAccountTransactionsResult = {
  transactions: AsyncResult<Transaction[]>;
  hasMore: boolean;
  loadMore: () => void;
};

export const DEFAULT_TRANSACTIONS_PAGE_SIZE = 50;

export const useAccountTransactions = (accountId: Id, pageSize = DEFAULT_TRANSACTIONS_PAGE_SIZE): UseAccountTransactionsResult => {
  const { flowRepository } = useRepositories();
  const [limit, setLimit] = useState(pageSize);
  const [transactions, setTransactions] = useState<AsyncResult<Transaction[]>>(Result.loading());
  const [totalCount, setTotalCount] = useState(0);

  // Reset the window whenever the account changes.
  useEffect(() => {
    setLimit(pageSize);
  }, [accountId, pageSize]);

  useEffect(() => {
    const unsubscribe = flowRepository.watchTransactions(setTransactions, accountId, limit);
    return () => unsubscribe();
  }, [flowRepository, accountId, limit]);

  useEffect(() => {
    const unsubscribe = flowRepository.watchTransactionCount((result) => {
      if (result.isSuccess) setTotalCount(result.value);
    }, accountId);
    return () => unsubscribe();
  }, [flowRepository, accountId]);

  const loadedCount = transactions.isSuccess ? transactions.value.length : 0;
  const hasMore = useMemo(() => loadedCount < totalCount, [loadedCount, totalCount]);

  // Idempotent: safe to call repeatedly (e.g. from an onAppear scroll trigger).
  // Only grows the window once the current page is fully loaded and more remain.
  const loadMore = useCallback(() => {
    setLimit((current) => {
      if (loadedCount < current) return current;
      if (loadedCount >= totalCount) return current;
      return current + pageSize;
    });
  }, [loadedCount, totalCount, pageSize]);

  return { transactions: transactions, hasMore: hasMore, loadMore: loadMore };
};
