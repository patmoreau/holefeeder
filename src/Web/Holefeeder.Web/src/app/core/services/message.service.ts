import { Injectable } from '@angular/core';
import { Observable, Subject } from 'rxjs';
import { Message } from '../models/message.model';

@Injectable({ providedIn: 'root' })
export class MessageService {
  private dispatcher = new Subject<Message>();

  sendMessage(message: Message) {
    this.dispatcher.next(message);
  }

  clearMessages() {
    this.dispatcher.next(Message.Empty);
  }

  get listen(): Observable<Message> {
    return this.dispatcher.asObservable();
  }
}
