import { Result } from './result';

export type Boolean = boolean & { readonly __brand: 'Boolean' };

export const BooleanErrors = {
  type: 'boolean-type',
};

const create = (value: unknown): Result<Boolean> => {
  if (typeof value !== 'boolean') return Result.failure([BooleanErrors.type]);
  return Result.success(value as Boolean);
};

const True = true as Boolean;
const False = false as Boolean;

export const Boolean = {
  create: create,
  True,
  False,
};
