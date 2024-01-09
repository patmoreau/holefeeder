import { inject } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { catchError, exhaustMap, map, of, switchMap } from 'rxjs';
import { StatisticsService } from './statistics.service';
import { StatisticsActions } from './statistics.actions';
import { SettingsService } from '@app/core/services';

export const fetchStatistics = createEffect(
  (
    actions$ = inject(Actions),
    statisticsService = inject(StatisticsService),
    settingsService = inject(SettingsService)
  ) => {
    return actions$.pipe(
      ofType(StatisticsActions.loadSummary),
      exhaustMap(() =>
        settingsService.period$.pipe(
          switchMap(period =>
            statisticsService.fetchSummary(period.start).pipe(
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
        )
      )
    );
  },
  { functional: true, dispatch: true }
);
