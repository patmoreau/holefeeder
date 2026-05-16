import { type AsyncResult, Result, Success } from '@/shared/core/result';

declare global {
  namespace jest {
    interface Matchers<R> {
      toBeSuccessWithValue(expected: any): R;
      toBeFailureWithErrors(expected: any): R;
      toBeLoading(): R;
    }
  }
}

declare global {
  var expectSuccess: <T>(result: Result<T> | AsyncResult<T>) => asserts result is Success<T>;
}

global.expectSuccess = expectSuccess;

function expectSuccess<T>(result: Result<T> | AsyncResult<T>): asserts result is Success<T> {
  if (result.isFailure) {
    throw new Error('Expected success but got failure', { cause: result.errors });
  }
}

expect.extend({
  toBeSuccessWithValue(received: Result<any> | AsyncResult<any>, expected: any) {
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

  toBeFailureWithErrors(received: Result<any> | AsyncResult<any>, expected: any) {
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

  toBeLoading(received: AsyncResult<any>, expected: any) {
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
