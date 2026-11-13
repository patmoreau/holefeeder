import type { CategoryType } from '@holefeeder/shared/core';
export type TransactionAccountOption = { id: string; name: string };

export type TransactionCategoryOption = { id: string; name: string; color: string; type: CategoryType };

export type NewTransactionOptions = { accounts: TransactionAccountOption[]; categories: TransactionCategoryOption[] };

const valid = (value: Record<string, unknown>): NewTransactionOptions => ({
  accounts: (value.accounts as Record<string, unknown>[]).map((account) => ({ id: account.id as string, name: account.name as string })),
  categories: (value.categories as Record<string, unknown>[]).map((category) => ({
    id: category.id as string,
    name: category.name as string,
    color: category.color as string,
    type: category.type as CategoryType,
  })),
});

export const NewTransactionOptions = { valid: valid };
