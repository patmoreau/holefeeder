import { type AsyncResult, Result } from '@holefeeder/shared/core';
import { TagInfo } from './tag-info';

export type TagsRepository = {
  watch: (onDataChange: (result: AsyncResult<TagInfo[]>) => void) => () => void;
  rename: (oldTag: string, newTag: string) => Promise<Result<void>>;
  remove: (tag: string) => Promise<Result<void>>;
};

export const TagsRepositoryErrors = {
  invalidTagName: 'invalid-tag-name',
};
