import { type AsyncResult } from '@holefeeder/shared/core';
import { TagInfo } from '../tag-info';
import { TagsRepository } from '../tags-repository';

export const WatchTagInfosUseCase = (repository: TagsRepository) => {
  const watch = (onDataChange: (result: AsyncResult<TagInfo[]>) => void) => repository.watch(onDataChange);

  return { watch: watch };
};
