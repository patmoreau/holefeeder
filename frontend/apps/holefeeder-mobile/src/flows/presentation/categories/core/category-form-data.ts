import { Id } from '@holefeeder/shared/core';
import { CategoryType } from '@/shared/core/category-type';

export type CategoryFormData = {
  id: Id | null;
  name: string;
  type: CategoryType;
  color: string;
  budgetAmount: number;
  favorite: boolean;
};
