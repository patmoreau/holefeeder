import {EMPTY, map, Observable, OperatorFunction, pipe, UnaryFunction} from 'rxjs';
import {ToastsService} from '../services/toasts.service';
import {LoadingBarService} from '@ngx-loading-bar/core';
import {ToastType} from '../models/toast-item.model';
import {Router} from '@angular/router';

export abstract class BaseResolverService {
  constructor(private router: Router, private toasts: ToastsService, protected barService: LoadingBarService) {
  }

  protected cancelWhenNullish<T>(message: string): UnaryFunction<Observable<T | null | undefined>, Observable<T>> {
    return pipe(
      map(x => {
        if (x === null || x === undefined) {
          this.toasts.show(ToastType.warning, message);
          this.barService.useRef('router').complete();
          this.router.navigateByUrl('/');
          return EMPTY;
        }
        return x;
      }) as OperatorFunction<T | null | undefined, T>
    )
  }
}
