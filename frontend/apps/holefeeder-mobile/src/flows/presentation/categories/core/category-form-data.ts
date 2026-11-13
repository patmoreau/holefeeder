import { CategoryType, Id } from '@holefeeder/shared/core';

export type CategoryFormData = {
  id: Id | null;
  name: string;
  type: CategoryType;
  color: string;
  budgetAmount: number;
  favorite: boolean;
};
