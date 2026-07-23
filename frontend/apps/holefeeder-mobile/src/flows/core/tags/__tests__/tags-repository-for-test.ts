import { type AsyncResult, Result } from '@holefeeder/shared/core';
import { TagInfo } from '@/flows/core/tags/tag-info';
import { TagsRepository } from '@/flows/core/tags/tags-repository';

export type TagsRepositoryInMemory = TagsRepository & {
  add: (...items: TagInfo[]) => void;
  isLoading: () => void;
  isFailing: (errors: string[]) => void;
};

export const TagsRepositoryInMemory = (): TagsRepositoryInMemory => {
  const itemsInMemory: TagInfo[] = [];
  let loadingInMemory = false;
  let errorsInMemory: string[] = [];

  const watch = (onDataChange: (result: AsyncResult<TagInfo[]>) => void) => {
    if (loadingInMemory) {
      onDataChange(Result.loading());
    } else if (errorsInMemory.length > 0) {
      onDataChange(Result.failure(errorsInMemory));
    } else {
      onDataChange(Result.success(itemsInMemory));
    }
    return () => {};
  };

  return {
    watch: watch,
    add: (...items: TagInfo[]) => itemsInMemory.push(...items),
    isLoading: () => (loadingInMemory = true),
    isFailing: (errors: string[]) => (errorsInMemory = errors),
  };
};
