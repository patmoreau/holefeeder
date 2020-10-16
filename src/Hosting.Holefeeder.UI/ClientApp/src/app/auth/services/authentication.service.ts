import { Injectable } from '@angular/core';
import { map, filter, catchError } from 'rxjs/operators';
import { Observable, BehaviorSubject } from 'rxjs';

import { ApiService } from '@app/shared/services/api.service';
import { IUser, userFromServer } from '@app/shared/interfaces/user.interface';
import { OidcSecurityService } from 'angular-auth-oidc-client';
import { UserAuthenticated } from '../enums/user-authenticated.enum';
import { User } from '@app/shared/models/user.model';

@Injectable({ providedIn: 'root' })
export class AuthenticationService {
  private basePath = 'api/v1/users';

  public authenticatedUser$ = new BehaviorSubject<IUser>(null);

  constructor(
    private oidcSecurityService: OidcSecurityService,
    private apiService: ApiService) {

    this.oidcSecurityService.isAuthenticated$.subscribe(async isAuth => {
      if (!isAuth) {
        this.authenticatedUser$.next(null);
        return;
      } else {
        const user = await this.getUserFromServer();
        this.authenticatedUser$.next(user);
      }
    });
  }

  get isAuthenticated$(): Observable<boolean> {
    return this.oidcSecurityService.isAuthenticated$;
  }

  async getUserFromServer(): Promise<IUser> {
    try {
      const user = await this.apiService
        .get(`${this.basePath}`)
        .pipe(
          map(userFromServer)
        ).toPromise();
      return user;
    } catch (error) {
      console.log('DrifterApps:err', error);
      return null;
    }
  }

  // public get User(): string {
  //   const claims = this.oauthService.getIdentityClaims();
  //   if (!claims) {
  //     return null;
  //   }
  //   return claims['given_name'];
  // }

  public get Token(): string {
    const token: string = this.oidcSecurityService.getIdToken();
    if (!token) {
      throw new Error('no token set , authentication required');
    }
    return token;
  }

  public login(): void {
    return this.oidcSecurityService.authorize();
  }

  public logout(): void {
    this.oidcSecurityService.logoff();
  }

  //   public get userValue(): User {
  //     return this.userSubject.value;
  // }

  // login(username, password) {
  //     return this.http.post<User>(`${environment.apiUrl}/users/authenticate`, { username, password })
  //         .pipe(map(user => {
  //             // store user details and jwt token in local storage to keep user logged in between page refreshes
  //             localStorage.setItem('user', JSON.stringify(user));
  //             this.userSubject.next(user);
  //             return user;
  //         }));
  // }

  // logout() {
  //     // remove user from local storage and set current user to null
  //     localStorage.removeItem('user');
  //     this.userSubject.next(null);
  //     this.router.navigate(['/account/login']);
  // }

  register(user: IUser) {
    return user;
  }

  // getAll() {
  //     return this.http.get<User[]>(`${environment.apiUrl}/users`);
  // }

  // getById(id: string) {
  //     return this.http.get<User>(`${environment.apiUrl}/users/${id}`);
  // }

  // update(id, params) {
  //     return this.http.put(`${environment.apiUrl}/users/${id}`, params)
  //         .pipe(map(x => {
  //             // update stored user if the logged in user updated their own record
  //             if (id == this.userValue.id) {
  //                 // update local storage
  //                 const user = { ...this.userValue, ...params };
  //                 localStorage.setItem('user', JSON.stringify(user));

  //                 // publish updated user to subscribers
  //                 this.userSubject.next(user);
  //             }
  //             return x;
  //         }));
  // }

  // delete(id: string) {
  //     return this.http.delete(`${environment.apiUrl}/users/${id}`)
  //         .pipe(map(x => {
  //             // auto logout if the logged in user deleted their own record
  //             if (id == this.userValue.id) {
  //                 this.logout();
  //             }
  //             return x;
  //         }));
  // }
}
