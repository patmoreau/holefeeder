import { Injectable } from '@angular/core';
import { MessageAction, MessageType } from '@app/shared/models';
import { Observable, Subject } from 'rxjs';
import { Message } from '../models';

@Injectable({ providedIn: 'root' })
export class MessageService {
  private dispatcher = new Subject<Message>();

  get listen(): Observable<Message> {
    return this.dispatcher.asObservable();
  }

  sendMessage({
    type,
    action,
    content,
  }: {
    type: MessageType;
    action: MessageAction;
    content?: any;
  }): void {
    this.dispatcher.next(new Message(type, action, content));
  }

  clearMessages() {
    this.dispatcher.next(Message.Empty);
  }
}
