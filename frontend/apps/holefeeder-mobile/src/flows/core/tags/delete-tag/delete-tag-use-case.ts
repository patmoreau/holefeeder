import { Result } from '@holefeeder/shared/core';
import { TagsRepository, TagsRepositoryErrors } from '@/flows/core/tags/tags-repository';

export const DeleteTagUseCase = (repository: TagsRepository) => {
  const execute = async (tag: string): Promise<Result<void>> => {
    const trimmed = tag.trim();
    if (trimmed.length === 0) {
      return Result.failure([TagsRepositoryErrors.invalidTagName]);
    }

    return repository.remove(trimmed);
  };

  return { execute: execute };
};
