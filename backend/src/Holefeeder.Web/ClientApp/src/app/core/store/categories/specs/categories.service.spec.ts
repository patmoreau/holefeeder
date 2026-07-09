import { provideHttpClient } from '@angular/common/http';
import {
  HttpTestingController,
  provideHttpClientTesting,
} from '@angular/common/http/testing';
import { TestBed, inject } from '@angular/core/testing';
import { Category } from '@app/shared/models';
import { CategoriesService } from '../categories.service';

const baseUrl = 'categories';

describe('CategoriesService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        provideHttpClient(),
        provideHttpClientTesting(),
        CategoriesService,
        { provide: 'BASE_API_URL', useValue: baseUrl },
      ],
    });
  });

  it('should be created', inject(
    [CategoriesService],
    (service: CategoriesService) => {
      expect(service).toBeTruthy();
    }
  ));

  it('should fetch categories', inject(
    [CategoriesService, HttpTestingController],
    (service: CategoriesService, httpMock: HttpTestingController) => {
      // Mock response data
      const mockResponse = [
        {
          id: 1,
          name: 'Category 1',
          description: 'Description 1',
          // ... Add more properties here as needed
        },
        // Add more mock data as needed
      ];

      // Access the private apiUrl property using bracket notation
      const apiUrl = (service as CategoriesService)['apiUrl'];

      // Make the HTTP request
      service.fetch().subscribe((categories: ReadonlyArray<Category>) => {
        // Check if the response matches the expected data
        expect(categories.length).toBe(mockResponse.length);

        // Add more assertions as needed to verify the data transformation

        // Verify that there are no outstanding requests
        httpMock.verify();
      });

      // Expect a single HTTP GET request to the specified API URL
      const req = httpMock.expectOne(`${apiUrl}/categories`); // Use apiUrl here
      expect(req.request.method).toBe('GET');

      // Respond to the request with mock data
      req.flush(mockResponse);
    }
  ));
});
