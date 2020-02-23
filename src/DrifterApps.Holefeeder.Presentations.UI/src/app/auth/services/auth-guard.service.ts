import { Injectable } from '@angular/core';
import {
  Router,
  CanActivate,
  ActivatedRouteSnapshot,
  RouterStateSnapshot,
  CanActivateChild
} from '@angular/router';
import { OAuthService } from 'angular-oauth2-oidc';

@Injectable({
  providedIn: 'root',
})
export class AuthGuardService implements CanActivate, CanActivateChild {

  constructor(private oauthService: OAuthService, private router: Router) { }

  async canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Promise<boolean> {
    const redirectUrl = route['_routerState']['url'];

    if (this.oauthService.hasValidIdToken()) {
      return true;
    }

    this.router.navigateByUrl(
      this.router.createUrlTree(
        ['/login'], {
        queryParams: {
          redirectUrl
        }
      }
      )
    );

    return false;
  }

  async canActivateChild(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Promise<boolean> {
    return this.canActivate(route, state);
  }
}
