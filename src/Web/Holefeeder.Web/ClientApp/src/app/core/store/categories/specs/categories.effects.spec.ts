import { Observable, of, take, throwError } from 'rxjs';
import { CategoriesActions } from '../categories.actions';
import { TestBed } from '@angular/core/testing';
import { Action } from '@ngrx/store';
import { fetchCategories } from '../categories.effects';
import { CategoriesService } from '../categories.service';
import { Category, CategoryType } from '@app/shared/models';
import { provideMockActions } from '@ngrx/effects/testing';

describe('Categories Effects', (): void => {
  let categoriesServiceMock: CategoriesService;
  let actions$: Observable<Action>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        provideMockActions(() => actions$),
        { provide: CategoriesService, useValue: categoriesServiceMock }, // Provide the mock CategoriesService
      ],
    });
  });

  it('should dispatch loadCategoriesSuccess action on successful fetch', done => {
    const mockCategories: ReadonlyArray<Category> = [
      {
        id: '1',
        name: 'Category 1',
        type: CategoryType.expense,
        color: '',
        budgetAmount: 123,
        favorite: false,
      },
    ];
    categoriesServiceMock = {
      fetch: () => of(mockCategories),
    } as unknown as CategoriesService;

    actions$ = of(CategoriesActions.loadCategories());

    TestBed.runInInjectionContext((): void => {
      fetchCategories(actions$, categoriesServiceMock)
        .pipe(take(1))
        .subscribe(action => {
          expect(action).toEqual(
            CategoriesActions.loadCategoriesSuccess({
              categories: mockCategories,
            })
          );
          done();
        });
    });
  });

  it('should dispatch failure action when categories fetch failed', done => {
    categoriesServiceMock = {
      fetch: () => throwError(() => new Error('Error message')),
    } as unknown as CategoriesService;

    actions$ = of(CategoriesActions.loadCategories());

    TestBed.runInInjectionContext((): void => {
      fetchCategories(actions$, categoriesServiceMock)
        .pipe(take(1))
        .subscribe(action => {
          expect(action).toEqual(
            CategoriesActions.loadCategoriesFailure({
              error: 'Error message',
            })
          );
          done();
        });
    });
  });
});
