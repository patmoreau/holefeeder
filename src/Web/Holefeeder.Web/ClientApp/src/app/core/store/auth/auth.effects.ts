import { Actions, createEffect, ofType } from '@ngrx/effects';
import { inject } from '@angular/core';
import { from, map, of, tap } from 'rxjs';
import { AuthActions } from './auth.actions';
import { switchMap } from 'rxjs/operators';
import { Router } from '@angular/router';
import { AuthService } from './services/auth.service';

export const checkAuth = createEffect(
  (actions$ = inject(Actions), authService = inject(AuthService)) =>
    actions$.pipe(
      ofType(AuthActions.checkAuth),
      switchMap(() =>
        authService
          .checkAuth()
          .pipe(
            map(isAuthenticated =>
              AuthActions.checkAuthComplete({ isAuthenticated })
            )
          )
      )
    ),
  { functional: true, dispatch: true }
);

export const checkAuthComplete = createEffect(
  (actions$ = inject(Actions), authService = inject(AuthService)) =>
    actions$.pipe(
      ofType(AuthActions.checkAuthComplete),
      map(action => action.isAuthenticated),
      switchMap(isAuthenticated => {
        if (!isAuthenticated) {
          return of(AuthActions.logoutComplete());
        }
        return authService.userData.pipe(
          map(profile =>
            AuthActions.loginComplete({ profile, isAuthenticated })
          )
        );
      })
    ),
  { functional: true, dispatch: true }
);

export const login = createEffect(
  (actions$ = inject(Actions), authService = inject(AuthService)) =>
    actions$.pipe(
      ofType(AuthActions.login),
      switchMap(() => authService.doLogin())
    ),
  { functional: true, dispatch: false }
);

export const logout = createEffect(
  (actions$ = inject(Actions), authService = inject(AuthService)) =>
    actions$.pipe(
      ofType(AuthActions.logout),
      switchMap(() => authService.signOut()),
      switchMap(() => of(AuthActions.logoutComplete()))
    ),
  { functional: true, dispatch: true }
);

export const logoutComplete = createEffect(
  (actions$ = inject(Actions), router = inject(Router)) =>
    actions$.pipe(
      ofType(AuthActions.logoutComplete),
      tap(() => from(router.navigate(['/'])))
    ),
  { functional: true, dispatch: false }
);
