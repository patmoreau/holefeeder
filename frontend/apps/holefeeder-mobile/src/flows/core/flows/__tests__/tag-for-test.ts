import { aCount, anId, aString } from '@holefeeder/shared/testkit';
import { Tag } from '@/flows/core/flows/tag';

const defaultTag = (): Tag => ({
  tag: aString(),
  count: aCount(),
  categoryId: anId(),
});

export const aTag = (overrides?: Partial<Tag>): Tag => ({
  ...defaultTag(),
  ...overrides,
});
