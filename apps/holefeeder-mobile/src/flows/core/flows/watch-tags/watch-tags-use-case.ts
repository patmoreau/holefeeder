import { type AsyncResult } from '@/shared/core/result';
import { FlowsRepository } from '../flows-repository';
import { Tag } from '../tag';

export const WatchTagsUseCase = (repository: FlowsRepository) => {
  const watch = (onDataChange: (result: AsyncResult<Tag[]>) => void) =>
    repository.watchTags((result: AsyncResult<Tag[]>) => {
      if (result.isLoading || result.isFailure) {
        onDataChange(result);
        return;
      }

      onDataChange(result);
    });

  return {
    watch: watch,
  };
};
