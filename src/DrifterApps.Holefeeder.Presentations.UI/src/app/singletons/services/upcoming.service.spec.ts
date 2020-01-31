import { TestBed, inject } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';

import { UpcomingService } from './upcoming.service';

describe('UpcomingService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [UpcomingService]
    });
  });

  it('should be created', inject(
    [UpcomingService],
    (service: UpcomingService) => {
      expect(service).toBeTruthy();
    }
  ));
});
