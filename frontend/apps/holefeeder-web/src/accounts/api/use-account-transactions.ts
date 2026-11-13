import { ApiClient } from '@holefeeder/shared/api';
import { type AsyncResult, Result } from '@holefeeder/shared/core';
import { useEffect, useEffectEvent, useState } from 'react';
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

type Loaded = { key: string; transactions: Result<AccountTransaction[]>; total: number };

export const useAccountTransactions = (accountId: string): AccountTransactions => {
  const auth = useAuthentication();
  const { version } = useApiRefresh();
  const [loaded, setLoaded] = useState<Loaded>();
  const [loadingMore, setLoadingMore] = useState(false);
  const signedIn = !!auth.user && !auth.isLoading;
  const key = `${version}:${accountId}`;

  const fetchPage = (offset: number) =>
    ApiClient(auth, apiConfig).getPage<Record<string, unknown>>(
      `/api/v2/accounts/${accountId}/transactions?offset=${offset}&limit=${pageSize}`
    );

  const fetchFirstPage = useEffectEvent(() => fetchPage(0));

  useEffect(() => {
    if (!signedIn) return;

    let current = true;
    void fetchFirstPage().then((response) => {
      if (!current) return;
      setLoaded((previous) =>
        response.isSuccess
          ? { key: key, transactions: Result.success(AccountTransaction.validList(response.value.items)), total: response.value.total }
          : { key: key, transactions: response, total: previous?.total ?? 0 }
      );
    });

    return () => {
      current = false;
    };
  }, [key, signedIn]);

  const transactions: AsyncResult<AccountTransaction[]> =
    !loaded || (loaded.key !== key && !loaded.transactions.isSuccess) ? Result.loading() : loaded.transactions;
  const items = transactions.isSuccess ? transactions.value : [];
  const total = loaded?.total ?? 0;

  const loadMore = () => {
    setLoadingMore(true);
    void fetchPage(items.length).then((response) => {
      setLoadingMore(false);
      setLoaded(
        response.isSuccess
          ? {
              key: key,
              transactions: Result.success([...items, ...AccountTransaction.validList(response.value.items)]),
              total: response.value.total,
            }
          : { key: key, transactions: response, total: total }
      );
    });
  };

  return { transactions: transactions, hasMore: items.length < total, loadingMore: loadingMore, loadMore: loadMore };
};
