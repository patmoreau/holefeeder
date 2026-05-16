export type Result<T> = Success<T> | Failure;
export type AsyncResult<T> = Result<T> | Loading;

export type Success<T> = {
  readonly isSuccess: true;
  readonly isFailure: false;
  readonly isLoading: false;
  readonly value: T;
};

export type Failure = {
  readonly isSuccess: false;
  readonly isFailure: true;
  readonly isLoading: false;
  readonly errors: string[];
};

export type Loading = {
  readonly isSuccess: false;
  readonly isFailure: false;
  readonly isLoading: true;
};

const LOADING: Loading = { isSuccess: false, isFailure: false, isLoading: true };

function success(): Success<void>;
function success<T>(value: T): Success<T>;
function success<T>(value?: T): Success<T> | Success<void> {
  return { isSuccess: true, isFailure: false, isLoading: false, value: value as T };
}

const failure = (errors: string[]): Failure => ({ isSuccess: false, isFailure: true, isLoading: false, errors });

const loading = (): Loading => LOADING;

function combine<T extends Record<string, unknown>>(results: { [K in keyof T]: Result<T[K]> }): Result<T>;
function combine<T extends Record<string, unknown>>(results: { [K in keyof T]: AsyncResult<T[K]> }): AsyncResult<T>;
function combine<T extends Record<string, unknown>>(results: {
  [K in keyof T]: AsyncResult<T[K]>;
}): AsyncResult<T> {
  let loadings: Loading[] = [];
  let failures: string[] = [];
  const combined = {} as T;
  for (const [key, result] of Object.entries(results)) {
    if (result.isLoading) {
      loadings.push(result);
    } else if (result.isFailure) {
      failures = failures.concat(result.errors);
    } else {
      combined[key as keyof T] = result.value;
    }
  }

  if (loadings.length > 0) return loading();
  return failures.length > 0 ? failure(failures) : success(combined);
}

function combineArray<T>(results: Result<T>[]): Result<T[]>;
function combineArray<T>(results: AsyncResult<T>[]): AsyncResult<T[]>;
function combineArray<T>(results: AsyncResult<T>[]): AsyncResult<T[]> {
  let loadings: Loading[] = [];
  const failures: string[] = [];
  const values: T[] = [];

  for (const result of results) {
    if (result.isLoading) {
      loadings.push(result);
    } else if (result.isFailure) {
      failures.push(...result.errors);
    } else {
      values.push(result.value);
    }
  }

  if (loadings.length > 0) return loading();
  return failures.length > 0 ? failure(failures) : success(values);
}

export const Result = {
  success: success,
  failure: failure,
  loading: loading,
  combine: combine,
  combineArray: combineArray,
} as const;
