import { inject, Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import {
  AuthenticatedResult,
  LoginResponse,
  OidcSecurityService,
  UserDataResult,
} from 'angular-auth-oidc-client';
import { map } from 'rxjs/operators';
import { User } from '@app/shared/models';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private isAuthenticatedFlag = false;

  private oidcSecurityService = inject(OidcSecurityService);
  constructor() {
    this.oidcSecurityService.isAuthenticated$
      .pipe(map((result: AuthenticatedResult) => result.isAuthenticated))
      .subscribe(isAuthenticated => {
        this.isAuthenticatedFlag = isAuthenticated;
      });
  }

  get isAuthenticated(): boolean {
    return this.isAuthenticatedFlag;
  }

  get token() {
    return this.oidcSecurityService.getIdToken();
  }

  get userData(): Observable<User> {
    return this.oidcSecurityService.userData$.pipe(
      map(
        (result: UserDataResult) =>
          new User(
            result.userData.given_name,
            result.userData.family_name,
            result.userData.name,
            result.userData.sub
          )
      )
    );
  }

  checkAuth() {
    return this.oidcSecurityService
      .checkAuth()
      .pipe(map((result: LoginResponse) => result.isAuthenticated));
  }

  doLogin() {
    return of(this.oidcSecurityService.authorize());
  }

  signOut() {
    return this.oidcSecurityService.logoffAndRevokeTokens();
  }
}
