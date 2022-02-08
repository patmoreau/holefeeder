import { Injectable } from '@angular/core';
import { Observable, Subject } from 'rxjs';
import { MessageAction } from '../models/message-action.enum';
import { MessageType } from '../models/message-type.enum';
import { Message } from '../models/message.model';

@Injectable({ providedIn: 'root' })
export class MessageService {
  private dispatcher = new Subject<Message>();

  sendMessage(type: MessageType, action: MessageAction, content?: any) {
    this.dispatcher.next(new Message(type, action, content));
  }

  clearMessages() {
    this.dispatcher.next(Message.Empty);
  }

  get listen(): Observable<Message> {
    return this.dispatcher.asObservable();
  }
}
