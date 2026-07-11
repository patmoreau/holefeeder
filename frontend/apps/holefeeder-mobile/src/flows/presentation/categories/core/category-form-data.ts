import { Id } from '@holefeeder/shared/core';
import { CategoryType } from '@/flows/core/categories/category-type';

export type CategoryFormData = {
  id: Id | null;
  name: string;
  type: CategoryType;
  color: string;
  budgetAmount: number;
  favorite: boolean;
};
