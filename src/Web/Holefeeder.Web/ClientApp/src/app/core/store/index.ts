// file location: store/index.ts
import { ActionReducer, ActionReducerMap } from '@ngrx/store';
import { AuthFeature, AuthState } from '@app/core/store/auth/auth.feature';
import * as authEffects from '@app/core/store/auth/auth.effects';
import { CategoriesFeature, CategoriesState } from '@app/core/store/categories';
import * as categoriesEffects from '@app/core/store/categories/categories.effects';
export * from './categories';

export interface AppState {
  auth: AuthState;
  categories: CategoriesState;
}

export const reducers: ActionReducerMap<AppState> = {
  auth: AuthFeature.reducer,
  categories: CategoriesFeature.reducer,
};

export interface AppStore {
  auth: ActionReducer<AuthState>;
  categories: ActionReducer<CategoriesState>;
}

export const appStore: AppStore = {
  auth: AuthFeature.reducer,
  categories: CategoriesFeature.reducer,
};

export const appEffects = [authEffects, categoriesEffects];
