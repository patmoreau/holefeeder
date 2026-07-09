import { Boolean, Result } from '@holefeeder/shared/core';

export type System = boolean & { readonly __brand: 'System' };

export const SystemErrors = {
  invalid: 'system-invalid',
};

const create = (value: unknown): Result<System> => {
  const result = Boolean.create(value);
  if (result.isFailure) return Result.failure([SystemErrors.invalid]);
  return Result.success<System>(result.value as unknown as System);
};

export const System = {
  create: create,
};
