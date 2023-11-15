import { inject } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { catchError, exhaustMap, map, of } from 'rxjs';
import { CategoriesService } from './categories.service';
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
