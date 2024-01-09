import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { StatisticsService, apiRoute, apiSummaryRoute } from '../statistics.service';
import { Statistics, Summary } from '@app/shared/models';
import { format } from 'date-fns';

const baseUrl = 'api/v2';

describe('StatisticsService', () => {
  let service: StatisticsService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [
        StatisticsService,
        { provide: 'BASE_API_URL', useValue: baseUrl },]
    });
    service = TestBed.inject(StatisticsService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  describe('find', () => {
    it('should return an Observable of Statistics[]', () => {
      const dummyStatistics: Statistics[] = [
        // Define your dummy statistics data here
      ];

      service.find().subscribe(statistics => {
        expect(statistics).toEqual(dummyStatistics);
      });

      const req = httpMock.expectOne(`${service.apiUrl}/${apiRoute}`);
      expect(req.request.method).toBe('GET');
      req.flush(dummyStatistics);
    });
  });

  describe('fetchSummary', () => {
    it('should return an Observable of Summary', () => {
      const dummySummary: Summary = {
        last: {  gains: 0, expenses: 0 },
        current: {  gains: 0, expenses: 0 },
        average: {  gains: 0, expenses: 0 },
      };

      const asOfDate = new Date();

      service.fetchSummary(asOfDate).subscribe(summary => {
        expect(summary).toEqual(dummySummary);
      });

      const req = httpMock.expectOne(`${service.apiUrl}/${apiSummaryRoute}?as-of=${format(asOfDate, 'yyyy-MM-dd')}`);
      expect(req.request.method).toBe('GET');
      req.flush(dummySummary);
    });
  });
});
