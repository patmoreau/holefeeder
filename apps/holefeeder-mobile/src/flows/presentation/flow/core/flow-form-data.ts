import { DateOnly, Id } from '@holefeeder/core';
import { Account } from '@/accounts/core/account';
import { Category } from '@/flows/core/categories/category';
import { Tag } from '@/flows/core/flows/tag';

export const FlowType = {
  expense: 'expense',
  gain: 'gain',
} as const;

export type FlowType = (typeof FlowType)[keyof typeof FlowType];

export type FlowFormData = {
  id: Id;
  flowType: FlowType;
  date: DateOnly;
  amount: number;
  description: string;
  account: Account;
  category: Category;
  tags: Tag[];
};
