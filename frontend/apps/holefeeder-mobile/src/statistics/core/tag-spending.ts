import { Money } from '@holefeeder/shared/core';

export type TagSpending = {
  tag: string;
  spentAmount: Money;
  avgAmount: Money;
};
