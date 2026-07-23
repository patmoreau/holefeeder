import { CategorySpending } from './category-spending';
import { CategoryTagSpending } from './category-tag-spending';

export type CombinedCategorySpending = {
  category: CategorySpending;
  tags: CategoryTagSpending[];
};
