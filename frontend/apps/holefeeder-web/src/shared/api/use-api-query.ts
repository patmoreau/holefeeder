import { ApiClient } from '@holefeeder/shared/api';
import { type AsyncResult, Result } from '@holefeeder/shared/core';
import { useEffect, useEffectEvent, useState } from 'react';
import { useAuthentication } from '@/shared/auth/authentication-context';
import { apiConfig } from './api-config';
import { useApiRefresh } from './api-refresh';

type Loaded<T> = { key: string; result: Result<T> };

export const useApiQuery = <TRaw, T>(endpoint: string, map: (raw: TRaw) => T): AsyncResult<T> => {
  const auth = useAuthentication();
  const { version } = useApiRefresh();
  const [loaded, setLoaded] = useState<Loaded<T>>();
  const signedIn = !!auth.user && !auth.isLoading;
  const key = `${version}:${endpoint}`;

  const load = useEffectEvent(async (): Promise<Result<T>> => {
    const response = await ApiClient(auth, apiConfig).get<TRaw>(endpoint);
    return response.isSuccess ? Result.success(map(response.value)) : response;
  });

  useEffect(() => {
    if (!signedIn) return;

    let current = true;
    void load().then((result) => {
      if (current) setLoaded({ key: key, result: result });
    });

    return () => {
      current = false;
    };
  }, [key, signedIn]);

  if (!loaded || (loaded.key !== key && !loaded.result.isSuccess)) return Result.loading();
  return loaded.result;
};
