import { TestBed } from '@angular/core/testing';
import { provideMockActions } from '@ngrx/effects/testing';
import { Observable, of, take, throwError } from 'rxjs';
import { StatisticsService } from '../statistics.service';
import { StatisticsActions } from '../statistics.actions';
import { fetchStatistics } from '../statistics.effects';
import { Action } from '@ngrx/store';
import { Summary } from '@app/shared/models';

describe('Statistics Effects', (): void => {
  let statisticsServiceMock: StatisticsService;
  let actions$: Observable<Action>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        provideMockActions(() => actions$),
        { provide: StatisticsService, useValue: statisticsServiceMock }, // Provide the mock StatisticsService
      ],
    });
  });

  it('should dispatch loadSummarySuccess action on successful fetch', done => {
    const mockSummary: Summary = {
      last: { gains: 0, expenses: 0 },
      current: { gains: 0, expenses: 0 },
      average: { gains: 0, expenses: 0 },
    };

    statisticsServiceMock = {
      fetchSummary: () => of(mockSummary),
    } as unknown as StatisticsService;

    actions$ = of(StatisticsActions.loadSummary());

    TestBed.runInInjectionContext((): void => {
      fetchStatistics(actions$, statisticsServiceMock)
        .pipe(take(1))
        .subscribe(action => {
          expect(action).toEqual(
            StatisticsActions.loadSummarySuccess({
              summary: mockSummary,
            })
          );
          done();
        });
    });
  });

  it('should dispatch loadSummaryFailure action when statistics fetch failed', done => {
    statisticsServiceMock = {
      fetchSummary: () => throwError(() => new Error('Error message')),
    } as unknown as StatisticsService;

    actions$ = of(StatisticsActions.loadSummary());

    TestBed.runInInjectionContext((): void => {
      fetchStatistics(actions$, statisticsServiceMock)
        .pipe(take(1))
        .subscribe(action => {
          expect(action).toEqual(
            StatisticsActions.loadSummaryFailure({
              error: 'Error message',
            })
          );
          done();
        });
    });
  });
});
