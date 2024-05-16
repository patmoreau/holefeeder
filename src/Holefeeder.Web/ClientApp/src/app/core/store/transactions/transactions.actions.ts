import { createActionGroup, emptyProps, props } from '@ngrx/store';
import { PagingInfo, TransactionDetail } from '@app/shared/models';

export const TransactionsActions = createActionGroup({
  source: 'Transactions API',
  events: {
    loadTransactions: props<{
      accountId: string;
      offset: number;
      limit: number;
      sort: string[];
    }>(),
    loadTransactionsSuccess: props<{
      transactions: PagingInfo<TransactionDetail>;
    }>(),
    loadTransactionsFailure: props<{ error: string }>(),
    clearTransactions: emptyProps(),
  },
});

// generated action creators:
/* eslint-disable @typescript-eslint/no-unused-vars */
const {
  loadTransactions,
  loadTransactionsSuccess,
  loadTransactionsFailure,
  clearTransactions,
} = TransactionsActions;
