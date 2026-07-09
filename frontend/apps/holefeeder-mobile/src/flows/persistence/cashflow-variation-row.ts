export type CashflowVariationRow = {
  id: string;
  accountId: string;
  lastPaidDate: string;
  lastCashflowDate: string;
  amount: number;
  description: string;
  effectiveDate: string;
  frequency: number;
  intervalType: string;
  categoryType: string;
  tags: string;
};
