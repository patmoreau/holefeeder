import { inject } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { catchError, exhaustMap, map, of } from 'rxjs';
import { TransactionsActions } from './transactions.actions';
import { TransactionsService } from './transactions.service';

export const fetchTransactions = createEffect(
  (
    actions$ = inject(Actions),
    transactionsService = inject(TransactionsService)
  ) => {
    return actions$.pipe(
      ofType(TransactionsActions.loadTransactions),
      exhaustMap(props =>
        transactionsService
          .fetch(props.accountId, props.offset, props.limit, props.sort)
          .pipe(
            map(transactions =>
              TransactionsActions.loadTransactionsSuccess({
                transactions: transactions,
              })
            ),
            catchError(error =>
              of(
                TransactionsActions.loadTransactionsFailure({
                  error: error.message,
                })
              )
            )
          )
      )
    );
  },
  { functional: true, dispatch: true }
);
