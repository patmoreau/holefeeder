import { ErrorHandler, Injectable } from '@angular/core';
import { logger } from '@app/core/logger';
import { MessageAction, MessageType } from '@app/shared/models';
import { MessageService } from '../services';

@Injectable({ providedIn: 'root' })
export class GlobalErrorHandler implements ErrorHandler {
  constructor(private messages: MessageService) {}

  handleError(error: Error) {
    logger.error(error);
    this.messages.sendMessage({
      type: MessageType.error,
      action: MessageAction.error,
      content: error.message,
    });
    throw error;
  }
}
