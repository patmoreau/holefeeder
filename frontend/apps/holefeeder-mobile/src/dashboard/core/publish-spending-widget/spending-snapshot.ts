export type SpendingTrendSnapshot = {
  amount: number;
  isOver: boolean;
};

export type SpendingSnapshot = {
  spending: number;
  trend?: SpendingTrendSnapshot;
  projected: number;
};
