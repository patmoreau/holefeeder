import { Id } from './id';
import { Result } from './result';
import { Validate, Validator } from './validate';

export type StoreItem = {
  id: Id;
  code: string;
  data: string;
};

export const StoreItemErrors = {
  invalidCode: 'invalid-code',
  invalidData: 'invalid-data',
};

const isValidJson = (value: unknown): Result<string> => {
  if (typeof value !== 'string') return Result.failure([StoreItemErrors.invalidData]);
  try {
    JSON.parse(value);
    return Result.success(value);
  } catch {
    return Result.failure([StoreItemErrors.invalidData]);
  }
};

const isValidCode = Validator.string({ minLength: 1 });

const create = (value: Record<string, unknown>): Result<StoreItem> =>
  Result.combine<StoreItem>({
    id: Id.create(value.id),
    code: Validate.validate(isValidCode, value.code, [StoreItemErrors.invalidCode]),
    data: isValidJson(value.data),
  });

const valid = (value: Record<string, unknown>): StoreItem => ({
  id: Id.valid(value.id),
  code: value.code as string,
  data: value.data as string,
});

export const StoreItem = {
  create: create,
  valid: valid,
};
