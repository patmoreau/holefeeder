import { Result } from '@holefeeder/shared/core';
import { TagsRepositoryErrors } from '@/flows/core/tags/tags-repository';

export type RenameTagCommand = {
  oldTag: string;
  newTag: string;
};

const create = (value: { oldTag: string; newTag: string }): Result<RenameTagCommand> => {
  const oldTag = value.oldTag.trim();
  const newTag = value.newTag.trim();

  if (oldTag.length === 0 || newTag.length === 0 || newTag.includes(',')) {
    return Result.failure([TagsRepositoryErrors.invalidTagName]);
  }

  return Result.success({ oldTag: oldTag, newTag: newTag });
};

export const RenameTagCommand = {
  create: create,
};
