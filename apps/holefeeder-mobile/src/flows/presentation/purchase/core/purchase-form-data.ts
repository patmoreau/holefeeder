import { DateIntervalType, DateOnly } from '@holefeeder/core';
import { Account } from '@/accounts/core/account';
import { Category } from '@/flows/core/categories/category';
import { Tag } from '@/flows/core/flows/tag';

export const PurchaseType = {
  expense: 'expense',
  gain: 'gain',
  transfer: 'transfer',
} as const;

export type PurchaseType = (typeof PurchaseType)[keyof typeof PurchaseType];

export type PurchaseFormData = {
  purchaseType: PurchaseType;
  date: DateOnly;
  amount: number;
  description: string;
  sourceAccount: Account;
  targetAccount: Account;
  category: Category;
  tags: Tag[];
  hasCashflow: boolean;
  cashflowEffectiveDate: DateOnly;
  cashflowIntervalType: DateIntervalType;
  cashflowFrequency: number;
};
