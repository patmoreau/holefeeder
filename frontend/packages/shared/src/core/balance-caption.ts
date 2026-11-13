export const BalanceCaptions = {
  availableBalance: 'availableBalance',
  amountDue: 'amountDue',
  creditBalance: 'creditBalance',
} as const;

export type BalanceCaption = (typeof BalanceCaptions)[keyof typeof BalanceCaptions];
