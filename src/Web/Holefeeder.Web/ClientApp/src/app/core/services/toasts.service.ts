import {Injectable} from '@angular/core';
import {StateService} from '@app/core/services/state.service';
import {MessageType} from '@app/shared/enums/message-type.enum';
import {filter, Observable} from 'rxjs';
import {MessageService} from './message.service';
import {ToastItem, ToastItemAdapter, ToastType} from "@app/core/models/toast-item.model";
import {MessageAction} from "@app/shared/enums/message-action.enum";
import {Message} from "@app/core/models/message.model";

interface ToastsState {
  toasts: ToastItem[];
}

const initialState: ToastsState = {
  toasts: [],
};

@Injectable({providedIn: 'root'})
export class ToastsService extends StateService<ToastsState> {
  toasts$: Observable<ToastItem[]> = this.select((state) => state.toasts);

  constructor(private messages: MessageService, private adapter: ToastItemAdapter) {
    super(initialState);

    this.messages.listen
      .pipe(
        filter(message => message.type === MessageType.error && message.action === MessageAction.error),
      ).subscribe((message: Message) => {
        this.setState({toasts: [...this.state.toasts, this.adapter.adapt({type: ToastType.danger, message: message.content})]});
      }
    );
  }

  show(type: ToastType, message: string): void {
    this.setState({toasts: [...this.state.toasts, this.adapter.adapt({type: type, message: message})]});
  }

  remove(toast: ToastItem): void {
    this.setState({toasts: [...this.state.toasts.filter(t => t !== toast)]});
  }
}
