import { createEntityAdapter, EntityState } from '@ngrx/entity';
import { CallState } from '@app/core/store/call-state.type';
import { TransactionDetail } from '@app/shared/models';
import { createFeature, createReducer, createSelector, on } from '@ngrx/store';
import { TransactionsActions } from '@app/core/store/transactions/transactions.actions';

export interface TransactionsState extends EntityState<TransactionDetail> {
  callState: CallState;
  error: string;
}

// adapter
export function selectId(a: TransactionDetail): string {
  return a.id;
}

export function sortByDate(
  ob1: TransactionDetail,
  ob2: TransactionDetail
): number {
  return +ob2.date - +ob1.date;
}

export const transactionAdapter = createEntityAdapter<TransactionDetail>({
  selectId: selectId,
  sortComparer: sortByDate,
});

export const initialState: TransactionsState =
  transactionAdapter.getInitialState({
    callState: 'init',
    error: '',
  });

export const TransactionsFeature = createFeature({
  name: 'transactions',
  reducer: createReducer(
    initialState,
    on(TransactionsActions.loadTransactions, state => ({
      ...state,
      callState: 'loading' as const,
    })),
    on(
      TransactionsActions.loadTransactionsSuccess,
      (state, { transactions }) => ({
        ...state,
        ...transactionAdapter.setAll(transactions.items.concat(), state),
        callState: 'loaded' as const,
      })
    ),
    on(TransactionsActions.loadTransactionsFailure, (state, { error }) => ({
      ...state,
      error,
      callState: 'loaded' as const,
    })),
    on(TransactionsActions.clearTransactions, state => ({
      ...state,
      ...transactionAdapter.removeAll(state),
      callState: 'init' as const,
    }))
  ),
  extraSelectors: ({ selectTransactionsState, selectCallState }) => ({
    ...transactionAdapter.getSelectors(selectTransactionsState),
    selectIsLoading: createSelector(
      selectCallState,
      callState => callState === 'loading'
    ),
  }),
});
