// file location: store/index.ts
import { ActionReducer, ActionReducerMap } from '@ngrx/store';
import { AuthFeature, AuthState } from '@app/core/store/auth/auth.feature';
import * as authEffects from '@app/core/store/auth/auth.effects';
import { CategoriesFeature, CategoriesState } from '@app/core/store/categories';
import * as categoriesEffects from '@app/core/store/categories/categories.effects';
import { TagsFeature, TagsState } from '@app/core/store/tags';
import * as tagsEffects from '@app/core/store/tags/tags.effects';
import { StatisticsFeature, StatisticsState } from '@app/core/store/statistics';
import * as statisticsEffects from '@app/core/store/statistics/statistics.effects';
export * from './categories';
export * from './tags';

export interface AppState {
  auth: AuthState;
  categories: CategoriesState;
  statistics: StatisticsState;
  tags: TagsState;
}

export const reducers: ActionReducerMap<AppState> = {
  auth: AuthFeature.reducer,
  categories: CategoriesFeature.reducer,
  statistics: StatisticsFeature.reducer,
  tags: TagsFeature.reducer,
};

export interface AppStore {
  auth: ActionReducer<AuthState>;
  categories: ActionReducer<CategoriesState>;
  statistics: ActionReducer<StatisticsState>;
  tags: ActionReducer<TagsState>;
}

export const appStore: AppStore = {
  auth: AuthFeature.reducer,
  categories: CategoriesFeature.reducer,
  statistics: StatisticsFeature.reducer,
  tags: TagsFeature.reducer,
};

export const appEffects = [authEffects, categoriesEffects, tagsEffects, statisticsEffects];
