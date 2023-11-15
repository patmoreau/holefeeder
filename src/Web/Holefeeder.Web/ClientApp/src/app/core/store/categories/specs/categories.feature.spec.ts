import { CategoriesActions } from '../categories.actions';
import { Category, CategoryType } from '@app/shared/models';
import {
  CategoriesFeature,
  CategoriesState,
  categoryAdapter,
  initialState,
} from '../categories.feature';

describe('Categories feature', () => {
  it('[Categories API] Load Categories', () => {
    // arrange
    const action = CategoriesActions.loadCategories();

    const expectedState: CategoriesState = {
      ...initialState,
      callState: 'loading',
    };

    // act
    const result = CategoriesFeature.reducer(initialState, action);

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
      ...initialState,
      ...categoryAdapter.setAll(categories.concat(), initialState),
      callState: 'loaded',
    };

    // act
    const result = CategoriesFeature.reducer(initialState, action);

    // assert
    expect(result).toEqual(expectedState);
  });

  it('[Categories API] Load Categories Failure', () => {
    // arrange
    const error = 'error';
    const action = CategoriesActions.loadCategoriesFailure({ error });

    const expectedState: CategoriesState = {
      ...initialState,
      error,
      callState: 'loaded',
    };

    // act
    const result = CategoriesFeature.reducer(initialState, action);

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
      ...initialState,
      ...categoryAdapter.setAll(categories.concat(), initialState),
      callState: 'loaded',
    };
    const action = CategoriesActions.clearCategories();

    const expectedState: CategoriesState = {
      ...initialState,
      callState: 'init',
    };

    // act
    const result = CategoriesFeature.reducer(state, action);

    // assert
    expect(result).toEqual(expectedState);
  });
});
