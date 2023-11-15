import { TestBed, inject } from '@angular/core/testing';
import { TransactionsService } from '../transactions.service';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { TransactionDetailAdapter } from '@app/core/adapters';

// write test for TransactionsService here
// mock response data
const mockResponse = [
  {
    id: 1,
    name: 'Category 1',
    description: 'Description 1',
    // ... Add more properties here as needed
  },
  // Add more mock data as needed
];

const baseUrl = 'api/v2/transactions';

describe('TransactionsService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [
        TransactionsService,
        TransactionDetailAdapter,
        { provide: 'BASE_API_URL', useValue: baseUrl },
      ],
    });
  });

  it('should be created', inject(
    [TransactionsService],
    (service: TransactionsService) => {
      expect(service).toBeTruthy();
    }
  ));

  // it('should fetch transactions', inject(
  //   [TransactionsService, HttpTestingController],
  //   (service: TransactionsService, httpMock: HttpTestingController) => {
  //     // Access the private apiUrl property using bracket notation
  //     const apiUrl = (service as TransactionsService)['apiUrl'];
  //
  //     // Make the HTTP request
  //     service
  //       .fetch(accountId, offset, limit, sort)
  //       .subscribe((transactions: ReadonlyArray<TransactionDetail>) => {
  //         // Check if the response matches the expected data
  //         expect(transactions.length).toBe(mockResponse.length);
  //
  //         // Add more assertions as needed to verify the data transformation
  //
  //         // Verify that there are no outstanding requests
  //         httpMock.verify();
  //       });
  //
  //     // Expect a single HTTP GET request to the specified API URL
  //     const req = httpMock.expectOne(`${apiUrl}/api/v2/transactions`); // Use apiUrl here
  //     expect(req.request.method).toBe('GET');
  //   }
  // ));
});
