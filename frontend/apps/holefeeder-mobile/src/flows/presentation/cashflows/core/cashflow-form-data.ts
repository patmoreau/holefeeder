import { DateIntervalType, DateOnly, Id } from '@holefeeder/shared/core';
import { Account } from '@/accounts/core/account';
import { Category } from '@/flows/core/categories/category';
import { Tag } from '@/flows/core/flows/tag';

export type CashflowFormData = {
  id: Id;
  effectiveDate: DateOnly;
  amount: number;
  description: string;
  account: Account;
  category: Category;
  tags: Tag[];
  intervalType: DateIntervalType;
  frequency: number;
  recurrence: number;
};
