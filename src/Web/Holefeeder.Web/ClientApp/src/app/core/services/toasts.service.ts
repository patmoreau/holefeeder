import { inject, Injectable } from '@angular/core';
import { SubscriberService } from '@app/core/services';
import { tapTrace } from '@app/shared/helpers';
import {
  Message,
  MessageAction,
  MessageType,
  ToastItem,
  ToastType,
} from '@app/shared/models';
import { filter, Observable } from 'rxjs';
import { ToastItemAdapter } from '../adapters';
import { MessageService } from './message.service';
import { StateService } from './state.service';
import { LoggerService } from '@app/core/logger';

interface ToastsState {
  toasts: ToastItem[];
}

const initialState: ToastsState = {
  toasts: [],
};

@Injectable({ providedIn: 'root' })
export class ToastsService extends StateService<ToastsState> {
  private logger = inject(LoggerService);

  toasts$: Observable<ToastItem[]> = this.select(state => state.toasts).pipe(
    tapTrace(this.logger)
  );

  constructor(
    private messages: MessageService,
    private adapter: ToastItemAdapter,
    private subscriptions: SubscriberService
  ) {
    super(initialState);

    const subscription = this.messages.listen
      .pipe(
        tapTrace(this.logger),
        filter(
          message =>
            message.type === MessageType.error &&
            message.action === MessageAction.error
        )
      )
      .subscribe((message: Message) => {
        this.setState({
          toasts: [
            ...this.state.toasts,
            this.adapter.adapt({
              type: ToastType.danger,
              message: message.content as string,
              delay: 5,
            }),
          ],
        });
      });
    this.subscriptions.add(subscription);
  }

  show(type: ToastType, message: string): void {
    this.setState({
      toasts: [
        ...this.state.toasts,
        this.adapter.adapt({ type: type, message: message, delay: 5 }),
      ],
    });
  }

  remove(toast: ToastItem): void {
    this.setState({ toasts: [...this.state.toasts.filter(t => t !== toast)] });
  }
}
