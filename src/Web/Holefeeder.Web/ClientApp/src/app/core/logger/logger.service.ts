import { environment } from '@env/environment';

export enum LoggingLevel {
  None = 'None',
  Verbose = 'Verbose',
  Debug = 'Debug',
  Info = 'Info',
  Warning = 'Warning',
  Error = 'Error',
}

export interface Logger {
  error(message: any, ...optionalParams: any[]): void;
  warning(message: any, ...optionalParams: any[]): void;
  info(message: any, ...optionalParams: any[]): void;
  verbose(message: any, ...optionalParams: any[]): void;
  debug(message: any, ...optionalParams: any[]): void;
  log(loggingLevel: LoggingLevel, message: any, ...optionalParams: any[]): void;
}

export class ConsoleLogger implements Logger {
  private static Logger: Logger | undefined = undefined;

  constructor() {
    ConsoleLogger.Logger = this;
  }

  public static getInstance(): Logger {
    if (!ConsoleLogger.Logger) {
      ConsoleLogger.Logger = new ConsoleLogger();
    }
    return ConsoleLogger.Logger;
  }

  error(message: any, ...optionalParams: any[]): void {
    if (this.shouldLog(LoggingLevel.Error)) {
      console.error(message, ...optionalParams);
    }
  }

  warning(message: any, ...optionalParams: any[]): void {
    if (this.shouldLog(LoggingLevel.Warning)) {
      console.warn(message, ...optionalParams);
    }
  }

  info(message: any, ...optionalParams: any[]): void {
    if (this.shouldLog(LoggingLevel.Info)) {
      console.info(message, ...optionalParams);
    }
  }

  debug(message: any, ...optionalParams: any[]): void {
    if (this.shouldLog(LoggingLevel.Debug)) {
      console.debug(message, ...optionalParams);
    }
  }

  verbose(message: any, ...optionalParams: any[]): void {
    if (this.shouldLog(LoggingLevel.Verbose)) {
      console.log(message, ...optionalParams);
    }
  }

  log(
    loggingLevel: LoggingLevel,
    message: any,
    ...optionalParams: any[]
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
    if (environment.loggingLevel === LoggingLevel.None) {
      return false;
    } else if (environment.loggingLevel === LoggingLevel.Error) {
      return level === LoggingLevel.Error;
    } else if (environment.loggingLevel === LoggingLevel.Warning) {
      return level === LoggingLevel.Error || level === LoggingLevel.Warning;
    } else if (environment.loggingLevel === LoggingLevel.Info) {
      return (
        level === LoggingLevel.Error ||
        level === LoggingLevel.Warning ||
        level === LoggingLevel.Info
      );
    } else if (environment.loggingLevel === LoggingLevel.Debug) {
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
