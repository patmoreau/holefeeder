import { ErrorHandler, Injectable } from '@angular/core';
import { MessageAction, MessageType } from '@app/shared/models';
import { LoggerService } from '../logger/logger.service';
import { MessageService } from '../services';

@Injectable()
export class GlobalErrorHandler implements ErrorHandler {
  constructor(
    private messages: MessageService,
    private logger: LoggerService
  ) {}

  handleError(error: Error) {
    this.logger.logError(error);
    this.messages.sendMessage({
      type: MessageType.error,
      action: MessageAction.error,
      content: error.message,
    });
    throw error;
  }
}
