import { CategoryTypes } from '@holefeeder/shared/core';
import { aFutureDate, anAmount, anId, aString } from '@holefeeder/shared/testkit';
import { aTagList } from '@/flows/core/flows/__tests__/tag-list-for-test';
import { UpcomingFlow } from '@/flows/core/flows/upcoming-flow';

const defaultUpcomingFlow = (): UpcomingFlow => ({
  id: anId(),
  date: aFutureDate(),
  amount: anAmount(),
  description: aString(),
  tags: aTagList(),
  categoryType: CategoryTypes.expense,
  categoryName: aString(),
  categoryColor: '#3B82F6',
});

export const aUpcomingFlow = (overrides?: Partial<UpcomingFlow>): UpcomingFlow => ({
  ...defaultUpcomingFlow(),
  ...overrides,
});
