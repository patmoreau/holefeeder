// file location: store/index.ts
import { ActionReducer, ActionReducerMap } from '@ngrx/store';
import { AuthFeature, AuthState } from '@app/core/store/auth/auth.feature';
import * as authEffects from '@app/core/store/auth/auth.effects';
import { CategoriesFeature, CategoriesState } from '@app/core/store/categories';
import * as categoriesEffects from '@app/core/store/categories/categories.effects';
import { TagsFeature, TagsState } from '@app/core/store/tags';
import * as tagsEffects from '@app/core/store/tags/tags.effects';
export * from './categories';
export * from './tags';

export interface AppState {
  auth: AuthState;
  categories: CategoriesState;
  tags: TagsState;
}

export const reducers: ActionReducerMap<AppState> = {
  auth: AuthFeature.reducer,
  categories: CategoriesFeature.reducer,
  tags: TagsFeature.reducer,
};

export interface AppStore {
  auth: ActionReducer<AuthState>;
  categories: ActionReducer<CategoriesState>;
  tags: ActionReducer<TagsState>;
}

export const appStore: AppStore = {
  auth: AuthFeature.reducer,
  categories: CategoriesFeature.reducer,
  tags: TagsFeature.reducer,
};

export const appEffects = [authEffects, categoriesEffects, tagsEffects];
