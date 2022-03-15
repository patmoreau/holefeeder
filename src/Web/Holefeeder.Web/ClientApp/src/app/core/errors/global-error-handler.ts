import {ErrorHandler, Injectable} from '@angular/core';
import {MessageAction} from '@app/shared/enums/message-action.enum';
import {MessageType} from '@app/shared/enums/message-type.enum';
import {MessageService} from '../services/message.service';

@Injectable()
export class GlobalErrorHandler implements ErrorHandler {
  constructor(
    private messages: MessageService
  ) {
  }

  handleError(error: Error) {
    this.messages.sendMessage(MessageType.error, MessageAction.error, error.message);
  }
}
