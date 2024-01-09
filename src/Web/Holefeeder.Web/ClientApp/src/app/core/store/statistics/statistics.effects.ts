import { inject } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { catchError, exhaustMap, map, of } from 'rxjs';
import { StatisticsService } from './statistics.service';
import { StatisticsActions } from './statistics.actions';

export const fetchStatistics = createEffect(
  (
    actions$ = inject(Actions),
    statisticsService = inject(StatisticsService)
  ) => {
    return actions$.pipe(
      ofType(StatisticsActions.loadSummary),
      exhaustMap(() =>
        statisticsService.fetchSummary(new Date()).pipe(
          map(summary =>
            StatisticsActions.loadSummarySuccess({ summary: summary })
          ),
          catchError(error =>
            of(
              StatisticsActions.loadSummaryFailure({ error: error.message })
            )
          )
        )
      )
    );
  },
  { functional: true, dispatch: true }
);
