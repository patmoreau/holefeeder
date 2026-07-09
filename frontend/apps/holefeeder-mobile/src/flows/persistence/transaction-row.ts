export type TransactionRow = {
  id: string;
  date: string;
  amount: number;
  description: string;
  accountId: string;
  categoryId: string;
  categoryType: string;
  tags: string;
  cashflowId: string | null;
  cashflowDate: string | null;
};
