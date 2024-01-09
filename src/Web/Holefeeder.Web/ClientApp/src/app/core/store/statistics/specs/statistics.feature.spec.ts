import { StatisticsActions } from '../statistics.actions';
import { Summary, SummaryValue } from '@app/shared/models';
import {
  StatisticsFeature,
  StatisticsState,
  initialStatisticsState,
} from '../statistics.feature';

describe('Statistics feature', () => {
  it('[Statistics API] Load Statistics', () => {
    // arrange
    const action = StatisticsActions.loadSummary();

    const expectedState: StatisticsState = {
      ...initialStatisticsState,
      callState: 'loading',
    };

    // act
    const result = StatisticsFeature.reducer(initialStatisticsState, action);

    // assert
    expect(result).toEqual(expectedState);
  });

  it('[Statistics API] Load Summary Success', () => {
    // arrange
    const summary = new Summary(
      new SummaryValue(1, 4),
      new SummaryValue(2, 5),
      new SummaryValue(3, 6)
    );
    const action = StatisticsActions.loadSummarySuccess({ summary });

    const expectedState: StatisticsState = {
      ...initialStatisticsState,
      summary,
      callState: 'loaded',
    };

    // act
    const result = StatisticsFeature.reducer(initialStatisticsState, action);

    // assert
    expect(result).toEqual(expectedState);
  });

  it('[Statistics API] Load Summary Failure', () => {
    // arrange
    const error = 'error';
    const action = StatisticsActions.loadSummaryFailure({ error });

    const expectedState: StatisticsState = {
      ...initialStatisticsState,
      error,
      callState: 'loaded',
    };

    // act
    const result = StatisticsFeature.reducer(initialStatisticsState, action);

    // assert
    expect(result).toEqual(expectedState);
  });

  it('[Statistics API] Clear Summary', () => {
    // arrange
    const summary = new Summary(
      new SummaryValue(1, 4),
      new SummaryValue(2, 5),
      new SummaryValue(3, 6)
    );

    const state: StatisticsState = {
      ...initialStatisticsState,
      summary,
      callState: 'loaded',
    };
    const action = StatisticsActions.clearSummary();

    const expectedState: StatisticsState = {
      ...initialStatisticsState,
      callState: 'init',
    };

    // act
    const result = StatisticsFeature.reducer(state, action);

    // assert
    expect(result).toEqual(expectedState);
  });
});
