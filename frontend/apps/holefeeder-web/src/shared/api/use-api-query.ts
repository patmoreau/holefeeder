import { type AsyncResult, Result } from '@holefeeder/shared/core';
import { useEffect, useState } from 'react';
import { useAuthentication } from '@/shared/auth/authentication-context';
import { ApiClient } from './api-client';
import { apiConfig } from './api-config';
import { useApiRefresh } from './api-refresh';

export const useApiQuery = <TRaw, T>(endpoint: string, map: (raw: TRaw) => T): AsyncResult<T> => {
  const auth = useAuthentication();
  const { version } = useApiRefresh();
  const [result, setResult] = useState<AsyncResult<T>>(Result.loading());
  const signedIn = !!auth.user && !auth.isLoading;

  useEffect(() => {
    if (!signedIn) return;

    let current = true;
    setResult((previous) => (previous.isSuccess ? previous : Result.loading()));
    void ApiClient(auth, apiConfig)
      .get<TRaw>(endpoint)
      .then((response) => {
        if (current) setResult(response.isSuccess ? Result.success(map(response.value)) : response);
      });

    return () => {
      current = false;
    };
  }, [endpoint, signedIn, version]);

  return result;
};
