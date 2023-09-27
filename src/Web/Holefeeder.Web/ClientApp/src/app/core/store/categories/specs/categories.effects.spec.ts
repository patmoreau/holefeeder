import { TestBed } from '@angular/core/testing';
import { provideMockActions } from '@ngrx/effects/testing';
import { Observable, of } from 'rxjs';
import { cold, hot } from 'jasmine-marbles'; // You can use jasmine-marbles for testing observables
import { fetchCategories } from '../categories.effects';
import { CategoriesActions } from '../categories.actions';
import { CategoriesService } from '@app/core/store/categories/services/categories.service';
import { Category, CategoryType } from '@app/shared/models';

// Import your CategoriesService mock or use a real mock library
const categoriesServiceMock = {
  fetch: () => of([]), // Mock the fetch method to return an observable
};

describe('fetchCategories', () => {
  let actions: Observable<any>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        provideMockActions(() => actions),
        { provide: CategoriesService, useValue: categoriesServiceMock }, // Provide the mock CategoriesService
      ],
    });
  });

  it('should dispatch loadCategoriesSuccess action on successful fetch', () => {
    const categories: ReadonlyArray<Category> = [
      {
        id: '1',
        name: 'Category 1',
        type: CategoryType.expense,
        color: '',
        budgetAmount: 123,
        favorite: false,
      },
    ];

    // Create an action to trigger the effect
    const action = CategoriesActions.loadCategories();

    // Create a cold observable for the service response
    const response = cold('-a|', { a: categories });

    // Set up the service mock to return the response
    categoriesServiceMock.fetch = () => response;

    // Define the expected action to be dispatched
    const expectedAction = CategoriesActions.loadCategoriesSuccess({
      categories,
    });

    // Use jasmine-marbles to test the effect
    actions = hot('-a', { a: action });
    const expected = cold('-b', { b: expectedAction });

    expect(fetchCategories).toBeObservable(expected);
  });

  it('should dispatch loadCategoriesFailure action on fetch error', () => {
    const errorMessage = 'An error occurred';

    // Create an action to trigger the effect
    const action = CategoriesActions.loadCategories();

    // Create a cold observable for the service error
    const errorResponse = cold('-#|', {}, errorMessage);

    // Set up the service mock to return the error response
    categoriesServiceMock.fetch = () => errorResponse;

    // Define the expected action to be dispatched
    const expectedAction = CategoriesActions.loadCategoriesFailure({
      error: errorMessage,
    });

    // Use jasmine-marbles to test the effect
    actions = hot('-a', { a: action });
    const expected = cold('-b', { b: expectedAction });

    expect(fetchCategories).toBeObservable(expected);
  });
});
