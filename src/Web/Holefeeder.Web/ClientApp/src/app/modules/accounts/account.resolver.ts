import {Injectable} from '@angular/core';
import {ActivatedRouteSnapshot, Resolve, Router, RouterStateSnapshot} from '@angular/router';
import {catchError, delay, first, Observable, take, tap} from 'rxjs';
import {Account} from "@app/modules/accounts/models/account.model";
import {AccountsService} from "@app/modules/accounts/services/accounts.service";
import {filterNullish} from "@app/shared/rxjs.helper";

@Injectable()
export class AccountResolver implements Resolve<Account> {
  constructor(
    private accountsService: AccountsService,
    private router: Router
  ) {
  }

  resolve(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<any> {

    return this.accountsService.findById(route.params['accountId'])
      .pipe(
        filterNullish(),
        first(),
        tap(account => this.accountsService.selectAccount(account)),
        catchError((err) => this.router.navigateByUrl('/')));
  }
}
