import { Id } from '@/shared/core/id';
import { Result } from '@/shared/core/result';
import { Validate, Validator } from '@/shared/core/validate';

export type Tag = {
  tag: string;
  count: number;
  categoryId?: Id;
};

export const TagErrors = {
  invalidName: 'invalid-name',
  invalidCount: 'invalid-count',
};

const isValidTag = Validator.string({ minLength: 1 });
const isValidCount = Validator.number({ min: 0 });

const create = (value: Record<string, unknown>): Result<Tag> => {
  const categoryId = value.categoryId ? Id.create(value.categoryId as string) : undefined;
  return Result.combine<Tag>({
    tag: Validate.validate(isValidTag, value.tag, [TagErrors.invalidName]),
    count: Validate.validate(isValidCount, value.count, [TagErrors.invalidCount]),
    ...(categoryId ? { categoryId } : {}),
  });
};

const valid = (value: Record<string, unknown>): Tag => {
  const categoryId =
    value.category_id || value.categoryId ? Id.valid((value.category_id as string) ?? (value.categoryId as string)) : undefined;
  return {
    tag: value.tag as string,
    count: value.count as number,
    ...(categoryId ? { categoryId } : {}),
  };
};

export const Tag = {
  create: create,
  valid: valid,
};
