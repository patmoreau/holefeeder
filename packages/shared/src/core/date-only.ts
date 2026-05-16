import { isValid, parseISO } from 'date-fns';
import { Result } from './result';
import { Validate, Validator } from './validate';

export type DateOnly = string & { readonly __brand: 'DateOnly' };

export const DateOnlyErrors = {
  invalid: 'date-invalid',
};

const DATE_ONLY_PATTERN = /^\d{4}-\d{2}-\d{2}$/;
const isValidPattern = Validator.pattern<DateOnly>({ pattern: DATE_ONLY_PATTERN });

const create = (value: unknown): Result<DateOnly> => {
  const patternResult = Validate.validate<DateOnly>(isValidPattern, value, [DateOnlyErrors.invalid]);

  if (!patternResult.isSuccess) {
    return patternResult;
  }

  const stringValue = patternResult.value;

  return isValid(parseISO(stringValue)) ? Result.success(stringValue as DateOnly) : Result.failure([DateOnlyErrors.invalid]);
};

const valid = (value: unknown): DateOnly => value as DateOnly;

export const DateOnly = { create: create, valid: valid } as const;
