import type { TransactionAccountOption, TransactionCategoryOption } from './new-transaction-options';

export type TransactionDetail = {
  id: string;
  date: string;
  amount: number;
  description: string;
  tags: string[];
  category: TransactionCategoryOption;
  account: TransactionAccountOption;
};

const valid = (value: Record<string, unknown>): TransactionDetail => {
  const category = value.category as Record<string, unknown>;
  const account = value.account as Record<string, unknown>;
  return {
    id: value.id as string,
    date: value.date as string,
    amount: value.amount as number,
    description: value.description as string,
    tags: value.tags as string[],
    category: {
      id: category.id as string,
      name: category.name as string,
      color: category.color as string,
      type: category.type as TransactionCategoryOption['type'],
    },
    account: { id: account.id as string, name: account.name as string },
  };
};

export const TransactionDetail = { valid: valid };
