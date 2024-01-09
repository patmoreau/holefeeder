/* eslint-disable @typescript-eslint/no-unused-vars */
import { createActionGroup, emptyProps, props } from '@ngrx/store';
import { Summary } from '@app/shared/models';

export const StatisticsActions = createActionGroup({
  source: 'Statistics API',
  events: {
    loadSummary: emptyProps(),
    loadSummarySuccess: props<{ summary: Summary }>(),
    loadSummaryFailure: props<{ error: string }>(),
    clearSummary: emptyProps(),
  },
});

// generated action creators:
const {
  loadSummary,
  loadSummarySuccess,
  loadSummaryFailure,
  clearSummary,
} = StatisticsActions;
