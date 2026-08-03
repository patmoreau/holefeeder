export type CashflowRow = {
  id: string;
  effectiveDate: string;
  amount: number;
  intervalType: string;
  frequency: number;
  recurrence: number;
  description: string;
  accountId: string;
  categoryId: string;
  categoryType: string;
  inactive: number;
  tags: string;
};
