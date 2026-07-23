import { Id, Money } from '@holefeeder/shared/core';

export type CategoryTagSpending = {
  categoryId: Id;
  tag: string;
  spentAmount: Money;
};
