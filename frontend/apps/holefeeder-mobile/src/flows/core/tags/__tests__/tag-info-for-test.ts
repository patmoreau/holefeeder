import { TagInfo } from '@/flows/core/tags/tag-info';
import { aPositiveCount } from '@/shared/__tests__/number-for-test';
import { aString } from '@/shared/__tests__/string-for-test';

const defaultTagInfo = (): TagInfo => ({
  tag: aString(),
  transactionCount: aPositiveCount(),
  activeCashflowCount: aPositiveCount(),
});

export const aTagInfo = (overrides?: Partial<TagInfo>): TagInfo => ({
  ...defaultTagInfo(),
  ...overrides,
});
