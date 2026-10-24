import { type AsyncResult, Result } from '@holefeeder/shared/core';
import { useEffect, useState } from 'react';
import { useAuthentication } from '@/shared/auth/authentication-context';
import { ApiClient } from './api-client';

const apiConfig = {
  url: import.meta.env.VITE_API_BASE_URL as string,
  timeout: 10_000,
};

export const useApiQuery = <TRaw, T>(endpoint: string, map: (raw: TRaw) => T): AsyncResult<T> => {
  const auth = useAuthentication();
  const [result, setResult] = useState<AsyncResult<T>>(Result.loading());
  const signedIn = !!auth.user && !auth.isLoading;

  useEffect(() => {
    if (!signedIn) return;

    let current = true;
    setResult(Result.loading());
    void ApiClient(auth, apiConfig)
      .get<TRaw>(endpoint)
      .then((response) => {
        if (current) setResult(response.isSuccess ? Result.success(map(response.value)) : response);
      });

    return () => {
      current = false;
    };
  }, [endpoint, signedIn]);

  return result;
};
