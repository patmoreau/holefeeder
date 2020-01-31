import {
  CanActivate,
  ActivatedRouteSnapshot,
  RouterStateSnapshot,
  Router
} from '@angular/router';
import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { map, catchError } from 'rxjs/operators';

import { AccountsService } from '../../shared/services/accounts.service';

@Injectable({
  providedIn: 'root'
})
export class AccountRouteActivatorService implements CanActivate {
  constructor(
    private accountsService: AccountsService,
    private router: Router
  ) {}

  async canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Promise<boolean> {
    const account = await this.accountsService.findOneById(route.params['id']);

    if (account) {
      return true;
    }

    return false;
    // .pipe(map(a => {
    //   console.log('coming thru');
    //   if (a) {
    //     console.log('good account');
    //     return true;
    //   }
    // }), catchError(err => {
    //   console.log('this should have worked');
    //   this.router.navigate(['/404']);
    //   return of(false);
    // }));
  }
}
