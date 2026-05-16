import { type AsyncResult, type Result, type Success } from '../core/result';

declare global {
  // eslint-disable-next-line @typescript-eslint/no-namespace
  namespace jest {
    interface Matchers<R> {
      toBeSuccessWithValue(expected: unknown): R;
      toBeFailureWithErrors(expected: unknown): R;
      toBeLoading(): R;
    }
  }
}

declare global {
  var expectSuccess: <T>(result: Result<T> | AsyncResult<T>) => asserts result is Success<T>;
}

globalThis.expectSuccess = expectSuccess;

function expectSuccess<T>(result: Result<T> | AsyncResult<T>): asserts result is Success<T> {
  if (result.isFailure) {
    throw new Error('Expected success but got failure', { cause: result.errors });
  }
}

expect.extend({
  toBeSuccessWithValue(received: Result<unknown> | AsyncResult<unknown>, expected: unknown) {
    if (received.isFailure === undefined) {
      return {
        pass: false,
        message: () => `expected is not a Result: ${this.utils.printReceived(received)}`,
      };
    }

    if (received.isFailure) {
      return {
        pass: false,
        message: () => `expected ${this.utils.printReceived(received)} to be a success`,
      };
    }

    if (received.isLoading) {
      return {
        pass: false,
        message: () => `expected ${this.utils.printReceived(received)} to be a success`,
      };
    }

    const valueMatches = this.equals(received.value, expected);
    const diff = this.utils.diff(expected, received.value, {
      expand: this.expand,
    });

    return {
      pass: valueMatches,
      message: () =>
        `expected ${this.utils.printReceived(received.value)} to match object ${this.utils.printExpected(expected)}${diff ? `\nDifference: ${diff}` : ''}`,
    };
  },

  toBeFailureWithErrors(received: Result<unknown> | AsyncResult<unknown>, expected: unknown) {
    if (!received.isFailure) {
      return {
        pass: false,
        message: () => `expected ${this.utils.printReceived(received)} to be a failure`,
      };
    }

    const errorsMatch = this.equals(received.errors, expected);

    return {
      pass: errorsMatch,
      message: () => `expected failure to be ${this.utils.printReceived(expected)} but got ${this.utils.printReceived(received.errors)}`,
    };
  },

  toBeLoading(received: AsyncResult<unknown>) {
    if (!received.isLoading) {
      return {
        pass: false,
        message: () => `expected ${this.utils.printReceived(received)} to be loading`,
      };
    }

    return {
      pass: true,
      message: () => ``,
    };
  },
});
