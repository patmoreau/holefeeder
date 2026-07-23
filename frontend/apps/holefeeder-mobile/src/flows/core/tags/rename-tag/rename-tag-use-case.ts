import { Result } from '@holefeeder/shared/core';
import { RenameTagCommand } from '@/flows/core/tags/rename-tag/rename-tag-command';
import { TagsRepository } from '@/flows/core/tags/tags-repository';

export const RenameTagUseCase = (repository: TagsRepository) => {
  const execute = async (oldTag: string, newTag: string): Promise<Result<void>> => {
    const command = RenameTagCommand.create({ oldTag: oldTag, newTag: newTag });
    if (!command.isSuccess) {
      return Result.failure(command.errors);
    }

    return repository.rename(command.value.oldTag, command.value.newTag);
  };

  return { execute: execute };
};
