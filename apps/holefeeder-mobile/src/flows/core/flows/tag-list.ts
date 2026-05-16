import { Result } from '@/shared/core/result';
import { Validate, Validator } from '@/shared/core/validate';

export type TagList = string[] & { readonly __brand: 'TagList' };

const isString = Validator.string();
const isValidStringArray = Validator.array<string>(isString);

const create = (tags: unknown): Result<TagList> => {
  const tagsResult = Validate.validate<string[]>(isValidStringArray, tags);
  if (!tagsResult.isSuccess) return tagsResult;

  return Result.success(TagList.valid([...new Set(tagsResult.value.map((tag) => tag.trim()).filter((tag) => tag.length > 0))]));
};

const valid = (tags: unknown): TagList => tags as TagList;

const fromConcatenatedString = (tags: string): string[] => (tags.trim().length > 0 ? tags.split(',').map((tag) => tag.trim()) : []);
const toConcatenatedString = (tags: TagList): string => (tags.length > 0 ? tags.join(',') : '');

export const TagList = {
  create: create,
  valid: valid,
  fromConcatenatedString: fromConcatenatedString,
  toConcatenatedString: toConcatenatedString,
};
