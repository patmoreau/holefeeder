import { useApiCommand } from '@/shared/api/use-api-command';

export type RepeatIntervalType = 'daily' | 'weekly' | 'monthly' | 'yearly';

export type RepeatInput = { startDate: string; intervalType: RepeatIntervalType; frequency: number };

export type PurchaseInput = {
  date: string;
  amount: number;
  description: string;
  accountId: string;
  categoryId: string;
  tags: string[];
  repeat: RepeatInput | null;
};

export const useNewTransactionActions = () => {
  const command = useApiCommand();

  const recordPurchase = (purchase: PurchaseInput) =>
    command('/api/v2/transactions/record-purchase', {
      date: purchase.date,
      amount: purchase.amount.toFixed(2),
      description: purchase.description,
      accountId: purchase.accountId,
      categoryId: purchase.categoryId,
      tags: purchase.tags,
      ...(purchase.repeat && {
        repeat: {
          startDate: purchase.repeat.startDate,
          intervalType: purchase.repeat.intervalType,
          frequency: String(purchase.repeat.frequency),
        },
      }),
    });

  return { recordPurchase: recordPurchase };
};
