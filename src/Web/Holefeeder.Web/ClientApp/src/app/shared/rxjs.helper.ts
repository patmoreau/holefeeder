import {EMPTY, filter, map, Observable, OperatorFunction, pipe, take, tap, UnaryFunction} from "rxjs";

export function filterNullish<T>(): UnaryFunction<Observable<T | null | undefined>, Observable<T>> {
  return pipe(
    filter(x => x !== null && x !== undefined) as OperatorFunction<T | null | undefined, T>
  );
}

export function emptyIfNullish<T>(): UnaryFunction<Observable<T | null | undefined>, Observable<T>> {
  return pipe(
    map(x => (x === null || x === undefined) ? EMPTY : x) as OperatorFunction<T | null | undefined, T>
  );
}

export function errorIfNullish<T>(): UnaryFunction<Observable<T | null | undefined>, Observable<T>> {
  return pipe(
    tap(x => {
      if (x === null || x === undefined) {
        throw new Error('Nullish found');
      }
      return x;
    }) as OperatorFunction<T | null | undefined, T>
  );
}

export function filterTrue(): UnaryFunction<Observable<boolean | undefined>, Observable<boolean>> {
  return pipe(
    filter(x => x === true) as OperatorFunction<boolean | undefined, boolean>
  );
}
