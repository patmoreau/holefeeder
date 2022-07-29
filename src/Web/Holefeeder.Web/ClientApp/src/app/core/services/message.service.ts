import {Injectable} from '@angular/core';
import {Observable, Subject} from 'rxjs';
import {Message} from '@app/core';
import {MessageAction, MessageType} from "@app/shared";

@Injectable({providedIn: 'root'})
export class MessageService {
  private dispatcher = new Subject<Message>();

  get listen(): Observable<Message> {
    return this.dispatcher.asObservable();
  }

  sendMessage(type: MessageType, action: MessageAction, content?: any) {
    this.dispatcher.next(new Message(type, action, content));
  }

  clearMessages() {
    this.dispatcher.next(Message.Empty);
  }
}
