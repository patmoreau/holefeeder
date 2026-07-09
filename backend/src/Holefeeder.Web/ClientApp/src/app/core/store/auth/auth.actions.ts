import { createActionGroup, emptyProps, props } from '@ngrx/store';
import { User } from '@app/shared/models';

export const AuthActions = createActionGroup({
  source: 'Auth API',
  events: {
    checkAuth: emptyProps(),
    checkAuthComplete: props<{ isAuthenticated: boolean }>(),
    login: emptyProps(),
    loginComplete: props<{ profile: User; isAuthenticated: boolean }>(),
    logout: emptyProps(),
    logoutComplete: emptyProps(),
  },
});

// generated action creators:
/* eslint-disable @typescript-eslint/no-unused-vars */
const {
  checkAuth,
  checkAuthComplete,
  login,
  loginComplete,
  logout,
  logoutComplete,
} = AuthActions;
