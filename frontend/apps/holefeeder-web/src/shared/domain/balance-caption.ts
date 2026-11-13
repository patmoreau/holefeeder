import { type BalanceCaption, tk } from '@holefeeder/shared/core';

export const balanceCaptionKey: Record<BalanceCaption, string> = {
  availableBalance: tk.accountCard.availableBalance,
  amountDue: tk.accountCard.amountDue,
  creditBalance: tk.accountCard.creditBalance,
};
