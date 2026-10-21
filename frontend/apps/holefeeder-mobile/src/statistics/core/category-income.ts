import { Id, Money } from '@holefeeder/shared/core';

export type CategoryIncome = {
  categoryId: Id;
  categoryName: string;
  color: string;
  receivedAmount: Money;
  avgAmount: Money;
};
