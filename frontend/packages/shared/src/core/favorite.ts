import { Boolean } from './boolean';
import { Result } from './result';

export type Favorite = boolean & { readonly __brand: 'Favorite' };

export const FavoriteErrors = {
  invalid: 'favorite-invalid',
};

const create = (value: unknown): Result<Favorite> => {
  const result = Boolean.create(value);
  if (result.isFailure) return Result.failure([FavoriteErrors.invalid]);
  return Result.success<Favorite>(result.value as unknown as Favorite);
};

export const Favorite = {
  create: create,
};
