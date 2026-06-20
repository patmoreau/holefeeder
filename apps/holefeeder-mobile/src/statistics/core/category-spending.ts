import { Id, Money } from '@holefeeder/shared/core';

export type CategorySpending = {
  categoryId: Id;
  categoryName: string;
  color: string;
  budgetAmount: Money;
  spentAmount: Money;
};
