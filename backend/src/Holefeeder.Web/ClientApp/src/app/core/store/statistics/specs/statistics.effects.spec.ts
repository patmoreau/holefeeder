import { TestBed } from '@angular/core/testing';
import { SettingsService } from '@app/core/services';
import { Summary } from '@app/shared/models';
import { provideMockActions } from '@ngrx/effects/testing';
import { Action } from '@ngrx/store';
import { Observable, of, take, throwError } from 'rxjs';
import { StatisticsActions } from '../statistics.actions';
import { fetchStatistics } from '../statistics.effects';
import { StatisticsService } from '../statistics.service';

describe('Statistics Effects', (): void => {
  let settingsServiceMock: SettingsService;
  let statisticsServiceMock: StatisticsService;
  let actions$: Observable<Action>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        provideMockActions(() => actions$),
        { provide: SettingsService, useValue: settingsServiceMock }, // Provide the mock SettingsService
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

    const mockPeriod = { start: '2023-01-01', end: '2023-01-31' };

    settingsServiceMock = {
      period$: of(mockPeriod),
    } as unknown as SettingsService;

    statisticsServiceMock = {
      fetchSummary: () => of(mockSummary),
    } as unknown as StatisticsService;

    actions$ = of(StatisticsActions.loadSummary());

    TestBed.runInInjectionContext((): void => {
      fetchStatistics(actions$, statisticsServiceMock, settingsServiceMock)
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
    const mockPeriod = { start: '2023-01-01', end: '2023-01-31' };

    settingsServiceMock = {
      period$: of(mockPeriod),
    } as unknown as SettingsService;

    statisticsServiceMock = {
      fetchSummary: () => throwError(() => new Error('Error message')),
    } as unknown as StatisticsService;

    actions$ = of(StatisticsActions.loadSummary());

    TestBed.runInInjectionContext((): void => {
      fetchStatistics(actions$, statisticsServiceMock, settingsServiceMock)
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
