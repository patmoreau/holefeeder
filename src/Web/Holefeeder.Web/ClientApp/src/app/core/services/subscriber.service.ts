import { Injectable, OnDestroy } from '@angular/core';
import { trace } from '@app/core';
import { Subscription } from 'rxjs';

@Injectable({ providedIn: 'any' })
export class SubscriberService implements OnDestroy {
  private subscriptions: Subscription;

  constructor() {
    this.subscriptions = new Subscription();
  }

  @trace()
  add(subscription: Subscription) {
    this.subscriptions.add(subscription);
  }

  @trace()
  ngOnDestroy(): void {
    this.subscriptions.unsubscribe();
  }
}
