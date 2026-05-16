import { Result } from '@/shared/core/result';
import { Validate, Validator } from '@/shared/core/validate';

export const CategoryTypes = {
  expense: 'expense',
  gain: 'gain',
} as const;

export type CategoryType = (typeof CategoryTypes)[keyof typeof CategoryTypes];

export const CategoryTypeErrors = {
  invalid: 'category-type-invalid',
};

const normalizeCategoryType = (type: string): CategoryType => {
  const normalized = type.trim().toLowerCase();
  if (normalized === 'expense') return CategoryTypes.expense;
  if (normalized === 'gain') return CategoryTypes.gain;
  return CategoryTypes.expense;
};

const isValid = Validator.enum<CategoryType>({ values: CategoryTypes });

const create = (value: unknown): Result<CategoryType> => {
  let normalized = value;
  if (typeof value === 'string') {
    const candidate = value.trim().toLowerCase();
    if (Object.values(CategoryTypes).includes(candidate as CategoryType)) {
      normalized = candidate;
    }
  }

  const result = Validate.validate(isValid, normalized, [CategoryTypeErrors.invalid]);
  if (result.isSuccess) {
    return Result.success(normalized as CategoryType);
  }
  return result;
};

const valid = (value: unknown): CategoryType => {
  return normalizeCategoryType(value as string);
};

const multiplier = {
  [CategoryTypes.expense]: -1,
  [CategoryTypes.gain]: 1,
};

export const CategoryType = {
  create: create,
  valid: valid,
  multiplier: multiplier,
};
