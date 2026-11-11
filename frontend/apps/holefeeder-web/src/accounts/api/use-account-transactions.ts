import { type AsyncResult, Result } from '@holefeeder/shared/core';
import { useEffect, useState } from 'react';
import { ApiClient } from '@/shared/api/api-client';
import { apiConfig } from '@/shared/api/api-config';
import { useApiRefresh } from '@/shared/api/api-refresh';
import { useAuthentication } from '@/shared/auth/authentication-context';
import { AccountTransaction } from '../domain/account-transaction';

const pageSize = 20;

export type AccountTransactions = {
  transactions: AsyncResult<AccountTransaction[]>;
  hasMore: boolean;
  loadingMore: boolean;
  loadMore: () => void;
};

export const useAccountTransactions = (accountId: string): AccountTransactions => {
  const auth = useAuthentication();
  const { version } = useApiRefresh();
  const [transactions, setTransactions] = useState<AsyncResult<AccountTransaction[]>>(Result.loading());
  const [total, setTotal] = useState(0);
  const [loadingMore, setLoadingMore] = useState(false);
  const signedIn = !!auth.user && !auth.isLoading;

  const fetchPage = (offset: number) =>
    ApiClient(auth, apiConfig).getPage<Record<string, unknown>>(
      `/api/v2/accounts/${accountId}/transactions?offset=${offset}&limit=${pageSize}`
    );

  useEffect(() => {
    if (!signedIn) return;

    let current = true;
    setTransactions((previous) => (previous.isSuccess ? previous : Result.loading()));
    void fetchPage(0).then((response) => {
      if (!current) return;
      if (!response.isSuccess) {
        setTransactions(response);
        return;
      }
      setTotal(response.value.total);
      setTransactions(Result.success(AccountTransaction.validList(response.value.items)));
    });

    return () => {
      current = false;
    };
  }, [accountId, signedIn, version]);

  const loaded = transactions.isSuccess ? transactions.value : [];

  const loadMore = () => {
    setLoadingMore(true);
    void fetchPage(loaded.length).then((response) => {
      setLoadingMore(false);
      if (!response.isSuccess) {
        setTransactions(response);
        return;
      }
      setTotal(response.value.total);
      setTransactions(Result.success([...loaded, ...AccountTransaction.validList(response.value.items)]));
    });
  };

  return { transactions: transactions, hasMore: loaded.length < total, loadingMore: loadingMore, loadMore: loadMore };
};
