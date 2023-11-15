import { Observable, of, take } from 'rxjs';
import { AuthActions } from '../auth.actions';
import { TestBed } from '@angular/core/testing';
import { Action } from '@ngrx/store';
import { AuthService } from '../auth.service';
import { User } from '@app/shared/models';
import {
  checkAuth,
  checkAuthComplete,
  login,
  logout,
} from '@app/core/store/auth/auth.effects';

describe('Auth Effects', (): void => {
  let authServiceMock: AuthService;
  let actions$: Observable<Action>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [],
      providers: [],
    });
  });

  it('should dispatch complete action when auth check completed', (done): void => {
    authServiceMock = {
      checkAuth: () => of(true),
    } as unknown as AuthService;

    actions$ = of(AuthActions.checkAuth());

    TestBed.runInInjectionContext((): void => {
      checkAuth(actions$, authServiceMock)
        .pipe(take(1))
        .subscribe(action => {
          expect(action).toEqual(
            AuthActions.checkAuthComplete({
              isAuthenticated: true,
            })
          );
          done();
        });
    });
  });

  it('should dispatch login complete action when user is authenticated', (done): void => {
    const mockProfile = new User('Doe', 'John', 'John Doe', '1');
    authServiceMock = {
      get userData(): Observable<User> {
        return of(mockProfile);
      },
    } as unknown as AuthService;

    actions$ = of(AuthActions.checkAuthComplete({ isAuthenticated: true }));

    TestBed.runInInjectionContext((): void => {
      checkAuthComplete(actions$, authServiceMock)
        .pipe(take(1))
        .subscribe(action => {
          expect(action).toEqual(
            AuthActions.loginComplete({
              profile: mockProfile,
              isAuthenticated: true,
            })
          );
          done();
        });
    });
  });

  it('should dispatch logout complete action when user is not authenticated', (done): void => {
    authServiceMock = {} as unknown as AuthService;

    actions$ = of(AuthActions.checkAuthComplete({ isAuthenticated: false }));

    TestBed.runInInjectionContext((): void => {
      checkAuthComplete(actions$, authServiceMock)
        .pipe(take(1))
        .subscribe(action => {
          expect(action).toEqual(AuthActions.logoutComplete());
          done();
        });
    });
  });

  it('should do login when user is logging in', (done): void => {
    authServiceMock = {
      doLogin: () => of(void 0),
    } as unknown as AuthService;
    const doLoginSpy = spyOn(authServiceMock, 'doLogin').and.callThrough();

    actions$ = of(AuthActions.login());

    TestBed.runInInjectionContext((): void => {
      login(actions$, authServiceMock)
        .pipe(take(1))
        .subscribe(() => {
          expect(doLoginSpy).toHaveBeenCalledTimes(1);
          done();
        });
    });
  });

  it('should dispatch logout complete action when user is logged out', (done): void => {
    authServiceMock = {
      signOut: () => of(1),
    } as unknown as AuthService;
    const signOutSpy = spyOn(authServiceMock, 'signOut').and.callThrough();

    actions$ = of(AuthActions.logout());

    TestBed.runInInjectionContext((): void => {
      logout(actions$, authServiceMock)
        .pipe(take(1))
        .subscribe(action => {
          expect(signOutSpy).toHaveBeenCalledTimes(1);
          expect(action).toEqual(AuthActions.logoutComplete());
          done();
        });
    });
  });
});
