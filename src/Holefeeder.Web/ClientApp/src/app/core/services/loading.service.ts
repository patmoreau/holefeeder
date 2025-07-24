import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class LoadingService {
  private loadingSub = new BehaviorSubject<boolean>(false);
  loading$ = this.loadingSub.asObservable();

  private requestCount = 0;

  show() {
    this.requestCount++;
    if (this.loadingSub.value === false) {
      this.loadingSub.next(true);
    }
  }

  hide() {
    this.requestCount--;
    if (this.requestCount <= 0) {
      this.requestCount = 0;
      // Use setTimeout to avoid ExpressionChangedAfterItHasBeenCheckedError
      setTimeout(() => {
        if (this.loadingSub.value === true) {
          this.loadingSub.next(false);
        }
      }, 0);
    }
  }
}
