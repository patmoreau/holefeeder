import { TestBed, inject } from '@angular/core/testing';

import { AccountRouteActivatorService } from './account-route-activator.service';

describe('AccountRouteActivatorService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [AccountRouteActivatorService]
    });
    jest.mock('./accounts.service', () => {
      return jest.fn().mockImplementation(() => {
        return {
          getAccounts: () => {
            throw new Error('Test error');
          }
        };
      });
    });
  });

  it('should be created', inject([AccountRouteActivatorService], (service: AccountRouteActivatorService) => {
    expect(service).toBeTruthy();
  }));

});
