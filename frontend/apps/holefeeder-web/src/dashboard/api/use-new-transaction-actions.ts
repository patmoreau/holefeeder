import { useApiCommand } from '@/shared/api/use-api-command';

export type PurchaseInput = { date: string; amount: number; description: string; accountId: string; categoryId: string; tags: string[] };

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
    });

  return { recordPurchase: recordPurchase };
};
