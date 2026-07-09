import { createFeature, createReducer, on } from '@ngrx/store';
import { StatisticsActions } from './statistics.actions';
import { CallState } from '@app/core/store/call-state.type';
import { Summary, SummaryValue } from '@app/shared/models';

export interface StatisticsState {
  callState: CallState;
  summary: Summary;
  error: string;
}

export const initialStatisticsState: StatisticsState ={
  callState: 'init',
  summary: new Summary(new SummaryValue(0, 0), new SummaryValue(0, 0), new SummaryValue(0, 0)),
  error: '',
};

export const StatisticsFeature = createFeature({
  name: 'statistics',
  reducer: createReducer(
    initialStatisticsState,
    on(StatisticsActions.loadSummary, state => ({
      ...state,
      callState: 'loading' as const,
    })),
    on(StatisticsActions.loadSummarySuccess, (state, { summary }) => ({
      ...state,
      summary,
      callState: 'loaded' as const,
    })),
    on(StatisticsActions.loadSummaryFailure, (state, { error }) => ({
      ...state,
      error,
      callState: 'loaded' as const,
    })),
    on(StatisticsActions.clearSummary, state => ({
      ...state,
      summary: new Summary(new SummaryValue(0, 0), new SummaryValue(0, 0), new SummaryValue(0, 0)),
      callState: 'init' as const,
    }))
  ),
});
