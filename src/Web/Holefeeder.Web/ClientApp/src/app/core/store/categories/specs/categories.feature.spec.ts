import { CategoriesActions } from '../categories.actions';
import { Category, CategoryType } from '@app/shared/models';
import {
  CategoriesFeature,
  CategoriesState,
  categoryAdapter,
  initialCategoriesState,
} from '../categories.feature';

describe('Categories feature', () => {
  it('[Categories API] Load Categories', () => {
    // arrange
    const action = CategoriesActions.loadCategories();

    const expectedState: CategoriesState = {
      ...initialCategoriesState,
      callState: 'loading',
    };

    // act
    const result = CategoriesFeature.reducer(initialCategoriesState, action);

    // assert
    expect(result).toEqual(expectedState);
  });

  it('[Categories API] Load Categories Success', () => {
    // arrange
    const category = new Category(
      '1',
      'Category 1',
      CategoryType.expense,
      'Color 1',
      123.45,
      false
    );
    const categories = [category];
    const action = CategoriesActions.loadCategoriesSuccess({ categories });

    const expectedState: CategoriesState = {
      ...initialCategoriesState,
      ...categoryAdapter.setAll(categories.concat(), initialCategoriesState),
      callState: 'loaded',
    };

    // act
    const result = CategoriesFeature.reducer(initialCategoriesState, action);

    // assert
    expect(result).toEqual(expectedState);
  });

  it('[Categories API] Load Categories Failure', () => {
    // arrange
    const error = 'error';
    const action = CategoriesActions.loadCategoriesFailure({ error });

    const expectedState: CategoriesState = {
      ...initialCategoriesState,
      error,
      callState: 'loaded',
    };

    // act
    const result = CategoriesFeature.reducer(initialCategoriesState, action);

    // assert
    expect(result).toEqual(expectedState);
  });

  it('[Categories API] Clear Categories', () => {
    // arrange
    const category = new Category(
      '1',
      'Category 1',
      CategoryType.expense,
      'Color 1',
      123.45,
      false
    );
    const categories = [category];

    const state: CategoriesState = {
      ...initialCategoriesState,
      ...categoryAdapter.setAll(categories.concat(), initialCategoriesState),
      callState: 'loaded',
    };
    const action = CategoriesActions.clearCategories();

    const expectedState: CategoriesState = {
      ...initialCategoriesState,
      callState: 'init',
    };

    // act
    const result = CategoriesFeature.reducer(state, action);

    // assert
    expect(result).toEqual(expectedState);
  });
});
