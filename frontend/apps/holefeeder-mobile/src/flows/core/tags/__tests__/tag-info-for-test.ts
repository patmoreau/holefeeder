import { aPositiveCount, aString } from '@holefeeder/shared/testkit';
import { TagInfo } from '@/flows/core/tags/tag-info';

const defaultTagInfo = (): TagInfo => ({
  tag: aString(),
  transactionCount: aPositiveCount(),
  activeCashflowCount: aPositiveCount(),
});

export const aTagInfo = (overrides?: Partial<TagInfo>): TagInfo => ({
  ...defaultTagInfo(),
  ...overrides,
});
