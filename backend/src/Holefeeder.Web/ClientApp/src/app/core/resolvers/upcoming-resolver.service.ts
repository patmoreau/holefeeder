import { Injectable, inject } from '@angular/core';
import { ActivatedRouteSnapshot, Resolve, Router } from '@angular/router';
import { UpcomingService } from '@app/core/services';
import { Upcoming } from '@app/shared/models';
import { LoadingBarService } from '@ngx-loading-bar/core';
import { map, Observable, take } from 'rxjs';
import { ToastsService } from '../services/toasts.service';
import { BaseResolverService } from './base-resolver.service';

@Injectable({ providedIn: 'root' })
export class UpcomingResolverService
  extends BaseResolverService
  implements Resolve<Upcoming>
{
  private upcomingService = inject(UpcomingService);

  constructor() {
    const router = inject(Router);
    const toasts = inject(ToastsService);
    const barService = inject(LoadingBarService);

    super(router, toasts, barService);
  }

  resolve(route: ActivatedRouteSnapshot): Observable<Upcoming> {
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
