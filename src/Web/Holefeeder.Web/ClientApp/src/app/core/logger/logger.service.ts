import { ConfigService } from '@app/core/services';
import { Injectable } from '@angular/core';
import { LoggingLevel } from './logging-level.enum';

export interface Logger {
  error(message: unknown, ...optionalParams: unknown[]): void;

  warning(message: unknown, ...optionalParams: unknown[]): void;

  info(message: unknown, ...optionalParams: unknown[]): void;

  verbose(message: unknown, ...optionalParams: unknown[]): void;

  debug(message: unknown, ...optionalParams: unknown[]): void;

  log(
    loggingLevel: LoggingLevel,
    message: unknown,
    ...optionalParams: unknown[]
  ): void;
}

export class DecoratorLogger implements Logger {
  private static logger: Logger | undefined = undefined;
  protected loggingLevel: LoggingLevel = LoggingLevel.Info;

  constructor(private level: LoggingLevel) {
    this.loggingLevel = level;
    DecoratorLogger.logger = this;
  }

  public static getInstance(): Logger {
    if (!DecoratorLogger.logger) {
      return new DecoratorLogger(LoggingLevel.Info);
    }
    return DecoratorLogger.logger;
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

@Injectable({ providedIn: 'root' })
export class ConsoleLogger extends DecoratorLogger {
  constructor(config: ConfigService) {
    super(LoggingLevel.Info);

    config.loggingLevel$.subscribe(level => {
      console.info('ConsoleLogger logging level changed', level);
      this.loggingLevel = level;
    });
  }
}
