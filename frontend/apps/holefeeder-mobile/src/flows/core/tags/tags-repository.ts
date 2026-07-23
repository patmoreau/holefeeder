import { type AsyncResult } from '@holefeeder/shared/core';
import { TagInfo } from './tag-info';

export type TagsRepository = {
  watch: (onDataChange: (result: AsyncResult<TagInfo[]>) => void) => () => void;
};
