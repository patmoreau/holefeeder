import { CategoryTypes } from '@/flows/core/categories/category-type';
import { aTagList } from '@/flows/core/flows/__tests__/tag-list-for-test';
import { UpcomingFlow } from '@/flows/core/flows/upcoming-flow';
import { aFutureDate } from '@/shared/__tests__/date-for-test';
import { anAmount } from '@/shared/__tests__/number-for-test';
import { anId, aString } from '@/shared/__tests__/string-for-test';

const defaultUpcomingFlow = (): UpcomingFlow => ({
  id: anId(),
  date: aFutureDate(),
  amount: anAmount(),
  description: aString(),
  tags: aTagList(),
  categoryType: CategoryTypes.expense,
});

export const aUpcomingFlow = (overrides?: Partial<UpcomingFlow>): UpcomingFlow => ({
  ...defaultUpcomingFlow(),
  ...overrides,
});
