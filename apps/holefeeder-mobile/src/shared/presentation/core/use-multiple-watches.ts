import { type AsyncResult, Logger } from '@holefeeder/shared/core';
import { useEffect, useRef, useState } from 'react';
import { ErrorKey } from '@/shared/core/error-key';

const logger = Logger.create('use-multiple-watches');

type WatchHook<T, TDefault extends T | null | undefined = undefined> = {
  (): AsyncResult<T>;
  defaultValue?: TDefault;
};

type WatchHooks = Record<string, WatchHook<unknown, unknown>>;

type WatchHookValue<H> =
  H extends WatchHook<infer R, infer D> ? (D extends NonNullable<D> ? R : D extends null ? R | null : R | undefined) : never;

type MultiWatchResult<T extends WatchHooks> = {
  data: { [K in keyof T]: WatchHookValue<T[K]> };
  isLoading: boolean;
  errors: {
    showError: boolean;
    setShowError: (show: boolean) => void;
    error: ErrorKey;
  };
  results: { [K in keyof T]: AsyncResult<unknown> };
};

export const withDefault = <T>(hook: () => AsyncResult<T>, defaultValue: T): WatchHook<T, T> => {
  const wrapped = () => hook();
  (wrapped as WatchHook<T, T>).defaultValue = defaultValue;
  return wrapped as WatchHook<T, T>;
};

export const useMultipleWatches = <T extends WatchHooks>(hooks: T): MultiWatchResult<T> => {
  const [showError, setShowError] = useState(false);
  const registeredKeysRef = useRef<Set<string>>(new Set());
  const prevResultsRef = useRef<Record<string, AsyncResult<unknown>>>({});

  const results = Object.entries(hooks).reduce<Record<string, AsyncResult<unknown>>>((acc, [key, hook]) => {
    acc[key] = hook();
    return acc;
  }, {});

  useEffect(() => {
    // Log new watch registrations
    Object.keys(hooks).forEach((key) => {
      if (!registeredKeysRef.current.has(key)) {
        logger.debug(`Watch registered: "${key}"`);
        registeredKeysRef.current.add(key);
      }
    });

    // Log when watch data is received or updated (result reference changes on each PowerSync update)
    Object.entries(results).forEach(([key, result]) => {
      if (result.isSuccess && result !== prevResultsRef.current[key]) {
        logger.debug(`Watch data received: "${key}"`);
      }
    });

    prevResultsRef.current = { ...results };
  });

  const isLoading = Object.values(results).some((r) => r.isLoading);

  const errors = Object.values(results).flatMap((r) => (r.isFailure ? r.errors : []));
  if (errors.length > 0) logger.error(errors);

  const hasErrors = errors.length > 0;

  useEffect(() => {
    if (hasErrors) {
      setShowError(true);
    }
  }, [hasErrors]);

  const data = Object.entries(results).reduce<Record<string, unknown>>((acc, [key, result]) => {
    const hook = hooks[key];
    const hasDefault = 'defaultValue' in hook;
    acc[key] = result.isSuccess ? result.value : hasDefault ? hook.defaultValue : undefined;
    return acc;
  }, {});

  return {
    data: data as { [K in keyof T]: WatchHookValue<T[K]> },
    isLoading: isLoading,
    errors: {
      showError,
      setShowError,
      error: ErrorKey.saveFailed,
    },
    results: results as { [K in keyof T]: AsyncResult<unknown> },
  };
};
