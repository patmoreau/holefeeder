import { createReducer, on, createFeature, createSelector } from '@ngrx/store';
import { User } from '@app/shared/models';
import { AuthActions } from './auth.actions';

export interface AuthState {
  profile: User | null;
  isAuthenticated: boolean;
}

const initialState: AuthState = {
  isAuthenticated: false,
  profile: null,
};

export const AuthFeature = createFeature({
  name: 'auth',
  reducer: createReducer(
    initialState,
    on(AuthActions.loginComplete, (state, { profile, isAuthenticated }) => {
      return {
        ...state,
        profile,
        isAuthenticated,
      };
    }),
    on(AuthActions.logout, (state, {}) => {
      return {
        ...state,
        profile: null,
        isAuthenticated: false,
      };
    })
  ),
  extraSelectors: ({ selectIsAuthenticated }) => ({
    isAuthenticated: () => createSelector(() => selectIsAuthenticated),
  }),
});
