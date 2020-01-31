import { Injectable } from '@angular/core';
import { map, catchError } from 'rxjs/operators';
import { Observable, of } from 'rxjs';
import { JwtHelperService } from '@auth0/angular-jwt';

import { ApiService } from '@app/shared/services/api.service';
import { ILogin } from '../interfaces/login.interface';

@Injectable({ providedIn: 'root' })
export class AuthenticationService {
  private basePath = 'api/v1/auth';

  constructor(
    private apiService: ApiService,
    private jwtHelper: JwtHelperService
  ) {}

  login(login: ILogin): Observable<boolean> {
    return this.apiService.post(this.basePath, login)
      .pipe(
        map(response => {
          const token = (<any>response).token;
          console.log(token);
          localStorage.setItem('access_token', token);
          return true;
        }),
        catchError(() => {
          return of(false);
        })
      );
  }

  logout() {
    // remove user from local storage to log user out
    for (const key in localStorage) {
        if (localStorage[key] !== '') {
            localStorage.removeItem(key);
        }
    }
    // localStorage.removeItem('access_token');
    // localStorage.removeItem('access_token2');
  }

  isTokenExpired(): boolean {
    const token = localStorage.getItem('access_token');
    return !token || this.jwtHelper.isTokenExpired(token);
  }
}
