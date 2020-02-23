import { Injectable } from '@angular/core';
import { map, catchError, filter, switchMap, tap } from 'rxjs/operators';
import { Observable, of, BehaviorSubject } from 'rxjs';
import { JwtHelperService } from '@auth0/angular-jwt';

import { ApiService } from '@app/shared/services/api.service';
import { IUser, userFromServer } from '@app/shared/interfaces/user.interface';
import { OAuthService, OAuthErrorEvent, OAuthEvent } from 'angular-oauth2-oidc';

@Injectable({ providedIn: 'root' })
export class AuthenticationService {
  private basePath = 'api/v1/auth';

  public userAuthenticated$ = new BehaviorSubject<boolean>(false);
  public authenticatedUser$ = new BehaviorSubject<IUser>(null);

  constructor(
    private oauthService: OAuthService,
    private apiService: ApiService
  ) {
    this.oauthService.events.subscribe(async event => {
      if (event instanceof OAuthErrorEvent) {
        console.log(event);
      } else {
        if ((event.type === 'token_received' || event.type === 'token_refreshed')) {
          const profile = await this.oauthService.loadUserProfile() as any;

          console.log(profile);
          const user = await this.findOneById(profile.sub).toPromise();
          console.log(user);
          this.userAuthenticated$.next(true);
          this.authenticatedUser$.next(user);
          return;
        }
      }
      this.userAuthenticated$.next(false);
      // if (validIdToken) {
      //   const claims = this.oauthService.getIdentityClaims();
      //   if (claims) {
      //     const user = await this.apiService
      //       .get(`${this.basePath}/${claims['sub']}`)
      //       .pipe(

      //         map(userFromServer)
      //       ).toPromise();

      //     console.log(user);
      //     this.authenticatedUser$.next(user);
      //   }
      // }
      // this.authenticatedUser$.next(null);
    });
  }

  findOneById(id: number | string): Observable<IUser> {
    return this.apiService
      .get(`${this.basePath}/${id}`)
      .pipe(
        map(userFromServer)
      );
  }

  public get User(): string {
    const claims = this.oauthService.getIdentityClaims();
    if (!claims) {
      return null;
    }
    return claims['given_name'];
  }

  public get Token(): string {
    const token: string = this.oauthService.getIdToken();
    if (!token) {
      throw new Error('no token set , authentication required');
    }
    return token;
  }

  public async logIn(): Promise<boolean> {
    return await this.oauthService.tryLoginImplicitFlow();
  }

  public logOut(): void {
    this.oauthService.logOut();
  }
}
