/* eslint-disable @typescript-eslint/no-unused-vars */
import { createActionGroup, emptyProps, props } from '@ngrx/store';
import { Category } from '@app/shared/models';

// https://www.concretepage.com/ngrx/ngrx-entity-example

export const CategoriesActions = createActionGroup({
  source: 'Categories API',
  events: {
    loadCategories: emptyProps(),
    loadCategoriesSuccess: props<{ categories: ReadonlyArray<Category> }>(),
    loadCategoriesFailure: props<{ error: string }>(),
    // addCategory: props<{ category: Category }>(),
    // removeCategory: props<{ id: string }>(),
    // updateCategory: props<{ category: Category }>(),
    // upsertCategory: props<{ category: Category }>(),
    // addCategories: props<{ categories: ReadonlyArray<Category> }>(),
    // removeCategories: props<{ ids: ReadonlyArray<string> }>(),
    // updateCategories: props<{ categories: ReadonlyArray<Category> }>(),
    // upsertCategories: props<{ categories: ReadonlyArray<Category> }>(),
    clearCategories: emptyProps(),
    clearCategoriesSuccess: emptyProps(),
  },
});

// generated action creators:
const {
  loadCategories,
  loadCategoriesSuccess,
  loadCategoriesFailure,
  clearCategories,
} = CategoriesActions;
