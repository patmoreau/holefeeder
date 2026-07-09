import { Injectable, inject, isDevMode } from '@angular/core';
import { ConfigService } from '@app/core/services';
import { LoggingLevel } from '@app/shared/models/logging-level.enum';

interface ErrorContext {
  component?: string;
  action?: string;
  userAgent?: string;
  url?: string;
  timestamp?: Date;
  userId?: string;
}

declare global {
  interface Window {
    loggerService?: LoggerService;
  }
}

@Injectable({ providedIn: 'root' })
export class LoggerService {
  private loggingLevel: LoggingLevel = LoggingLevel.Info;
  private errorHistory: Array<{ error: Error; context?: ErrorContext }> = [];
  private maxHistorySize = 50;

  constructor() {
    const config = inject(ConfigService);

    this.loggingLevel = LoggingLevel.Info;

    config.loggingLevel$.subscribe(level => {
      console.info('ConsoleLogger logging level changed', level);
      this.loggingLevel = level;
    });

    // In development mode, expose logger service to window for console access
    if (isDevMode()) {
      window.loggerService = this;
      console.log('🔧 LoggerService available at window.loggerService');
    }
  }

  error(message: unknown, ...optionalParams: unknown[]): void {
    if (this.shouldLog(LoggingLevel.Error)) {
      console.error(message, ...optionalParams);
    }
  }

  warning(message: unknown, ...optionalParams: unknown[]): void {
    if (this.shouldLog(LoggingLevel.Warning)) {
      console.warn(message, ...optionalParams);
    }
  }

  info(message: unknown, ...optionalParams: unknown[]): void {
    if (this.shouldLog(LoggingLevel.Info)) {
      console.info(message, ...optionalParams);
    }
  }

  debug(message: unknown, ...optionalParams: unknown[]): void {
    if (this.shouldLog(LoggingLevel.Debug)) {
      console.debug(message, ...optionalParams);
    }
  }

  verbose(message: unknown, ...optionalParams: unknown[]): void {
    if (this.shouldLog(LoggingLevel.Verbose)) {
      console.log(message, ...optionalParams);
    }
  }

  log(
    loggingLevel: LoggingLevel,
    message: unknown,
    ...optionalParams: unknown[]
  ): void {
    if (this.shouldLog(loggingLevel)) {
      switch (loggingLevel) {
        case LoggingLevel.Error:
          this.error(message, ...optionalParams);
          break;
        case LoggingLevel.Warning:
          this.warning(message, ...optionalParams);
          break;
        case LoggingLevel.Info:
          this.info(message, ...optionalParams);
          break;
        case LoggingLevel.Debug:
          this.debug(message, ...optionalParams);
          break;
        case LoggingLevel.Verbose:
          this.verbose(message, ...optionalParams);
          break;
      }
    }
  }

  // Error tracking and debugging methods (merged from DebugService)
  logError(error: Error, context?: ErrorContext) {
    const errorEntry = {
      error,
      context: {
        ...context,
        userAgent: navigator.userAgent,
        url: window.location.href,
        timestamp: new Date(),
      }
    };

    this.errorHistory.unshift(errorEntry);

    // Keep only the most recent errors
    if (this.errorHistory.length > this.maxHistorySize) {
      this.errorHistory.splice(this.maxHistorySize);
    }

    if (isDevMode()) {
      console.group(`🐛 Error logged by LoggerService`);
      console.error('Error:', error);
      console.log('Context:', errorEntry.context);
      console.groupEnd();
    }
  }

  getErrorHistory() {
    return [...this.errorHistory];
  }

  clearErrorHistory() {
    this.errorHistory = [];
    if (isDevMode()) {
      console.log('🧹 Error history cleared');
    }
  }

  exportErrorReport(): string {
    const report = {
      timestamp: new Date().toISOString(),
      userAgent: navigator.userAgent,
      url: window.location.href,
      errors: this.errorHistory.map(entry => ({
        message: entry.error.message,
        stack: entry.error.stack,
        context: entry.context
      }))
    };

    return JSON.stringify(report, null, 2);
  }

  downloadErrorReport() {
    const report = this.exportErrorReport();
    const blob = new Blob([report], { type: 'application/json' });
    const url = URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.download = `error-report-${new Date().toISOString().split('T')[0]}.json`;
    link.click();
    URL.revokeObjectURL(url);
  }

  // Helper methods for console debugging
  logInfo(message: string, data?: unknown) {
    if (isDevMode()) {
      console.log(`ℹ️ ${message}`, data);
    }
  }

  logWarning(message: string, data?: unknown) {
    if (isDevMode()) {
      console.warn(`⚠️ ${message}`, data);
    }
  }

  logSuccess(message: string, data?: unknown) {
    if (isDevMode()) {
      console.log(`✅ ${message}`, data);
    }
  }

  private shouldLog(level: LoggingLevel): boolean {
    if (this.loggingLevel === LoggingLevel.None) {
      return false;
    } else if (this.loggingLevel === LoggingLevel.Error) {
      return level === LoggingLevel.Error;
    } else if (this.loggingLevel === LoggingLevel.Warning) {
      return level === LoggingLevel.Error || level === LoggingLevel.Warning;
    } else if (this.loggingLevel === LoggingLevel.Info) {
      return (
        level === LoggingLevel.Error ||
        level === LoggingLevel.Warning ||
        level === LoggingLevel.Info
      );
    } else if (this.loggingLevel === LoggingLevel.Debug) {
      return (
        level === LoggingLevel.Error ||
        level === LoggingLevel.Warning ||
        level === LoggingLevel.Info ||
        level === LoggingLevel.Debug
      );
    } else {
      return true;
    }
  }
}
