import { provideHttpClient } from '@angular/common/http';
import {
  HttpTestingController,
  provideHttpClientTesting,
} from '@angular/common/http/testing';
import { TestBed, inject } from '@angular/core/testing';
import { Tag } from '@app/shared/models/tag.model';
import { TagsService } from '../tags.service';

const baseUrl = 'tags';

describe('TagsService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        provideHttpClient(),
        provideHttpClientTesting(),
        TagsService,
        { provide: 'BASE_API_URL', useValue: baseUrl },
      ],
    });
  });

  it('should be created', inject([TagsService], (service: TagsService) => {
    expect(service).toBeTruthy();
  }));

  it('should fetch tags', inject(
    [TagsService, HttpTestingController],
    (service: TagsService, httpMock: HttpTestingController) => {
      // Mock response data
      const mockResponse = [
        {
          tag: 'tag name',
          count: 12,
        },
      ];

      // Access the private apiUrl property using bracket notation
      const apiUrl = (service as TagsService)['apiUrl'];

      // Make the HTTP request
      service.fetch().subscribe((tags: ReadonlyArray<Tag>) => {
        // Check if the response matches the expected data
        expect(tags.length).toBe(mockResponse.length);

        // Add more assertions as needed to verify the data transformation

        // Verify that there are no outstanding requests
        httpMock.verify();
      });

      // Expect a single HTTP GET request to the specified API URL
      const req = httpMock.expectOne(`${apiUrl}/tags`); // Use apiUrl here
      expect(req.request.method).toBe('GET');

      // Respond to the request with mock data
      req.flush(mockResponse);
    }
  ));
});
