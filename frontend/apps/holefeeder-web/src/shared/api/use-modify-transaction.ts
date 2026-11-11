import { useApiCommand } from '@/shared/api/use-api-command';

export type TransactionChanges = {
  id: string;
  date: string;
  amount: number;
  description: string;
  accountId: string;
  categoryId: string;
  tags: string[];
};

export const useModifyTransaction = () => {
  const command = useApiCommand();

  return (changes: TransactionChanges) =>
    command('/api/v2/transactions/modify', {
      id: changes.id,
      date: changes.date,
      amount: changes.amount.toFixed(2),
      description: changes.description,
      accountId: changes.accountId,
      categoryId: changes.categoryId,
      tags: changes.tags,
    });
};
