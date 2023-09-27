import { ErrorHandler, Injectable } from '@angular/core';
import { MessageAction, MessageType } from '@app/shared/models';
import { MessageService } from '../services';
import { ConsoleLogger } from '@app/core/logger/logger.service';

@Injectable({ providedIn: 'root' })
export class GlobalErrorHandler implements ErrorHandler {
  constructor(
    private messages: MessageService,
    private logger: ConsoleLogger
  ) {}

  handleError(error: Error) {
    this.logger.error(error);
    this.messages.sendMessage({
      type: MessageType.error,
      action: MessageAction.error,
      content: error.message,
    });
    throw error;
  }
}
