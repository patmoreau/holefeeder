import {ErrorHandler, Injectable} from '@angular/core';
import {MessageAction} from '@app/shared/enums/message-action.enum';
import {MessageType} from '@app/shared/enums/message-type.enum';
import {MessageService} from '../services/message.service';
import {LoggerService} from "@app/core/logger/logger.service";

@Injectable()
export class GlobalErrorHandler implements ErrorHandler {
  constructor(private messages: MessageService, private logger: LoggerService) {
  }

  handleError(error: Error) {
    this.logger.logError(error);
    this.messages.sendMessage(MessageType.error, MessageAction.error, error.message);
  }
}
