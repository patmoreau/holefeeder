import {Injectable} from "@angular/core";
import {Account, AccountsService} from "@app/core";
import {catchError, first, Observable, tap} from "rxjs";
import {ActivatedRouteSnapshot, Resolve, Router, RouterStateSnapshot} from "@angular/router";
import {filterNullish} from "@app/shared";
import {LoggerService} from "@app/core/logger/logger.service";

@Injectable({providedIn: 'root'})
export class AccountResolver implements Resolve<Account> {
  constructor(
    private accountsService: AccountsService,
    private logger: LoggerService,
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
