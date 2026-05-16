import { type AsyncResult, Result } from './result';

type Watcher<T> = (onDataChange: (result: AsyncResult<T>) => void) => () => void;

export const combineWatchers = <T extends any[], R>(watchers: { [K in keyof T]: Watcher<T[K]> }, selector: (...args: T) => R): Watcher<R> => {
  return (onDataChange: (result: AsyncResult<R>) => void) => {
    const values: AsyncResult<any>[] = new Array(watchers.length).fill(Result.loading());
    const hasEmitted = new Array(watchers.length).fill(false);
    const unsubscribes: (() => void)[] = [];

    const emit = () => {
      if (hasEmitted.some((emitted) => !emitted)) {
        onDataChange(Result.loading());
        return;
      }

      const errors: string[] = [];
      let isFailure = false;
      for (const res of values) {
        if (res.isFailure) {
          isFailure = true;
          if (res.errors) {
            errors.push(...res.errors);
          }
        }
      }
      if (isFailure) {
        onDataChange(Result.failure(errors));
        return;
      }

      if (values.some((res) => res.isLoading)) {
        onDataChange(Result.loading());
        return;
      }

      try {
        const successValues = values.map((res) => (res as any).value);
        const result = selector(...(successValues as T));
        onDataChange(Result.success(result));
      } catch (e: any) {
        onDataChange(Result.failure([e.message || 'Error in selector']));
      }
    };

    watchers.forEach((watcher, index) => {
      const unsub = watcher((result) => {
        values[index] = result;
        hasEmitted[index] = true;
        emit();
      });
      unsubscribes.push(unsub);
    });

    return () => {
      unsubscribes.forEach((unsub) => unsub());
    };
  };
};
