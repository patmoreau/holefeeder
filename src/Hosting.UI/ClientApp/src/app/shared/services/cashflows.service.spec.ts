import { TestBed, inject } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { CashflowsService } from './cashflows.service';

describe('CashflowsService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [CashflowsService]
    });
  });

  it('should be created', inject(
    [CashflowsService],
    (service: CashflowsService) => {
      expect(service).toBeTruthy();
    }
  ));
});
