import { ErrorHandler, Injectable } from '@angular/core';
import { MessageAction, MessageType } from '@app/shared/models';
import { MessageService } from '../services';
import { logger } from "@app/core";

@Injectable()
export class GlobalErrorHandler implements ErrorHandler {
  constructor(
    private messages: MessageService,
  ) {}

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
