import {ErrorHandler, Injectable} from '@angular/core';
import {MessageService} from '@app/core';
import {LoggerService} from "@app/core/logger/logger.service";
import {MessageAction, MessageType} from "@app/shared";

@Injectable()
export class GlobalErrorHandler implements ErrorHandler {
  constructor(private messages: MessageService, private logger: LoggerService) {
  }

  handleError(error: Error) {
    this.logger.logError(error);
    this.messages.sendMessage(MessageType.error, MessageAction.error, error.message);
    throw error;
  }
}
