import { Injectable } from '@angular/core';
import {
  ActivatedRouteSnapshot,
  Resolve,
  Router,
  RouterStateSnapshot,
} from '@angular/router';
import { Upcoming } from '@app/core/models/upcoming.model';
import { UpcomingService } from '@app/core/services/upcoming.service';
import { LoadingBarService } from '@ngx-loading-bar/core';
import { map, Observable, take } from 'rxjs';
import { ToastsService } from '../services/toasts.service';
import { BaseResolverService } from './base-resolver.service';

@Injectable({ providedIn: 'root' })
export class UpcomingResolverService
  extends BaseResolverService
  implements Resolve<Upcoming>
{
  constructor(
    private upcomingService: UpcomingService,
    router: Router,
    toasts: ToastsService,
    barService: LoadingBarService
  ) {
    super(router, toasts, barService);
  }

  resolve(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<Upcoming> {
    const id = route.params['cashflowId'];
    const date = new Date(route.queryParams['date']);
    return this.upcomingService.upcoming$.pipe(
      map((items: Upcoming[]) =>
        items.find(
          u => u.id === id && u.date.toISOString() === date.toISOString()
        )
      ),
      take(1),
      this.cancelWhenNullish('Cashflow is not found.')
    );
  }
}
