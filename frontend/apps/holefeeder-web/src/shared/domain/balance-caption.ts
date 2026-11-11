import { tk } from '@holefeeder/shared/core';

export const BalanceCaptions = {
  availableBalance: 'availableBalance',
  amountDue: 'amountDue',
  creditBalance: 'creditBalance',
} as const;

export type BalanceCaption = (typeof BalanceCaptions)[keyof typeof BalanceCaptions];

export const balanceCaptionKey: Record<BalanceCaption, string> = {
  availableBalance: tk.accountCard.availableBalance,
  amountDue: tk.accountCard.amountDue,
  creditBalance: tk.accountCard.creditBalance,
};
