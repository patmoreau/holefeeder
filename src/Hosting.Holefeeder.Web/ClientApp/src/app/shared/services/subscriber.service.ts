import {Injectable, OnDestroy} from '@angular/core';
import { Subscription } from 'rxjs';

@Injectable()
export class SubscriberService implements OnDestroy {
    private subscriptions: Subscription;

    constructor() {
        this.subscriptions = new Subscription();
    }

    add(subscription: Subscription) {
        this.subscriptions.add(subscription);
    }

    ngOnDestroy(): void {
        this.subscriptions.unsubscribe();
    }
}
