import { ErrorHandler, Injectable, isDevMode, inject } from '@angular/core';
import { MessageAction, MessageType } from '@app/shared/models';
import { MessageService } from '../services';
import { LoggerService } from '@app/core/logger/logger.service';

@Injectable({ providedIn: 'root' })
export class GlobalErrorHandler implements ErrorHandler {
  private messages = inject(MessageService);
  private logger = inject(LoggerService);


  handleError(error: Error) {
    // Log the error for debugging and tracking
    this.logger.error(error);
    this.logger.logError(error, {
      component: 'GlobalErrorHandler',
      action: 'handleError'
    });

    // In development mode, also log to console for easier debugging
    if (isDevMode()) {
      console.group('🚨 Global Error Handler');
      console.error('Error caught by GlobalErrorHandler:', error);
      console.error('Stack trace:', error.stack);
      console.log('Use window.loggerService.getErrorHistory() to see all errors');
      console.groupEnd();
    }

    // Show user-friendly message
    this.messages.sendMessage({
      type: MessageType.error,
      action: MessageAction.error,
      content: this.getDisplayMessage(error),
    });

    // Don't re-throw the error to prevent console crashes
    // In production, errors should be handled gracefully
  }

  private getDisplayMessage(error: Error): string {
    // In development, show the actual error message
    if (isDevMode()) {
      return error.message || 'An unexpected error occurred';
    }

    // In production, show a generic message unless it's a known user-facing error
    if (error.message && this.isUserFacingError(error)) {
      return error.message;
    }

    return 'An unexpected error occurred. Please try again or contact support if the problem persists.';
  }

  private isUserFacingError(error: Error): boolean {
    // Add logic here to determine if an error should be shown to users
    // For example, validation errors, network errors, etc.
    const userFacingErrorTypes = [
      'ValidationError',
      'NetworkError',
      'AuthenticationError',
      'AuthorizationError'
    ];

    return userFacingErrorTypes.some(type =>
      error.constructor.name === type ||
      error.message.includes(type)
    );
  }
}
