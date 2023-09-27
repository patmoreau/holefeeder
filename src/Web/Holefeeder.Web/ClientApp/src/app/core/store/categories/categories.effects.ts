import { inject } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { catchError, exhaustMap, map, of } from 'rxjs';
import { CategoriesService } from './services/categories.service';
import { CategoriesActions } from './categories.actions';

export const fetchCategories = createEffect(
  (
    actions$ = inject(Actions),
    categoriesService = inject(CategoriesService)
  ) => {
    return actions$.pipe(
      ofType(CategoriesActions.loadCategories),
      exhaustMap(() =>
        categoriesService.fetch().pipe(
          map(categories =>
            CategoriesActions.loadCategoriesSuccess({ categories: categories })
          ),
          catchError(error =>
            of(
              CategoriesActions.loadCategoriesFailure({ error: error.message })
            )
          )
        )
      )
    );
  },
  { functional: true, dispatch: true }
);

export const clearCategories = createEffect(
  (actions$ = inject(Actions)) => {
    return actions$.pipe(
      ofType(CategoriesActions.clearCategories),
      map(() => CategoriesActions.clearCategoriesSuccess())
    );
  },
  { functional: true, dispatch: true }
);
