import { Boolean } from './boolean';
import { Result } from './result';

export type Inactive = boolean & { readonly __brand: 'Inactive' };

export const InactiveErrors = {
  invalid: 'inactive-invalid',
};

const create = (value: unknown): Result<Inactive> => {
  const result = Boolean.create(value);
  if (result.isFailure) return Result.failure([InactiveErrors.invalid]);
  return Result.success<Inactive>(result.value as unknown as Inactive);
};

export const Inactive = {
  create: create,
};
