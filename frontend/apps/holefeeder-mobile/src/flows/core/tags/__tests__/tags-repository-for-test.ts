import { type AsyncResult, Result } from '@holefeeder/shared/core';
import { TagInfo } from '@/flows/core/tags/tag-info';
import { TagsRepository } from '@/flows/core/tags/tags-repository';

export type TagsRepositoryInMemory = TagsRepository & {
  add: (...items: TagInfo[]) => void;
  isLoading: () => void;
  isFailing: (errors: string[]) => void;
  renames: () => { oldTag: string; newTag: string }[];
  removed: () => string[];
};

export const TagsRepositoryInMemory = (): TagsRepositoryInMemory => {
  const itemsInMemory: TagInfo[] = [];
  const renamesInMemory: { oldTag: string; newTag: string }[] = [];
  const removedInMemory: string[] = [];
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

  const rename = async (oldTag: string, newTag: string) => {
    if (errorsInMemory.length > 0) {
      return Result.failure(errorsInMemory);
    }
    renamesInMemory.push({ oldTag: oldTag, newTag: newTag });
    return Result.success();
  };

  const remove = async (tag: string) => {
    if (errorsInMemory.length > 0) {
      return Result.failure(errorsInMemory);
    }
    removedInMemory.push(tag);
    return Result.success();
  };

  return {
    watch: watch,
    rename: rename,
    remove: remove,
    add: (...items: TagInfo[]) => itemsInMemory.push(...items),
    isLoading: () => (loadingInMemory = true),
    isFailing: (errors: string[]) => (errorsInMemory = errors),
    renames: () => renamesInMemory,
    removed: () => removedInMemory,
  };
};
