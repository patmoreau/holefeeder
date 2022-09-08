import { Injectable } from '@angular/core';
import { ConfigService, LoggingLevel } from '@app/core/services/config.service';

@Injectable({ providedIn: 'root' })
export class LoggerService {
  private _level: LoggingLevel = LoggingLevel.None;

  constructor(config: ConfigService) {
    config.loggingLevel$.subscribe(
      (loggingLevel: LoggingLevel) => (this._level = loggingLevel)
    );
    this.log = this.log.bind(this);
    this.logError = this.logError.bind(this);
    this.logWarning = this.logWarning.bind(this);
    this.logInfo = this.logInfo.bind(this);
    this.logVerbose = this.logVerbose.bind(this);
    this.shouldLog = this.shouldLog.bind(this);
  }

  private log(
    message: any,
    level = LoggingLevel.Warning,
    ...optionalParams: any[]
  ): void {
    if (this.shouldLog(level)) {
      switch (level) {
        case LoggingLevel.Errors:
          console.error(message, optionalParams);
          break;
        case LoggingLevel.Warning:
          console.warn(message, optionalParams);
          break;
        case LoggingLevel.Info:
          console.info(message, optionalParams);
          break;
        default:
          console.debug(message, optionalParams);
      }
    }
  }

  logError(message: any, ...optionalParams: any[]): void {
    console.log(message);
    this.log(message, LoggingLevel.Errors, optionalParams);
  }

  logWarning(message: any, ...optionalParams: any[]): void {
    console.log(message);
    this.log(message, LoggingLevel.Warning, optionalParams);
  }

  logInfo(message: any, ...optionalParams: any[]): void {
    console.log(message);
    this.log(message, LoggingLevel.Info, optionalParams);
  }

  logVerbose(message: any, ...optionalParams: any[]): void {
    console.log(message);
    this.log(message, LoggingLevel.Verbose, optionalParams);
  }

  private shouldLog(level: LoggingLevel): boolean {
    if (this._level === LoggingLevel.None) {
      return false;
    } else if (this._level === LoggingLevel.Errors) {
      return level === LoggingLevel.Errors;
    } else if (this._level === LoggingLevel.Warning) {
      return level === LoggingLevel.Errors || level === LoggingLevel.Warning;
    } else if (this._level === LoggingLevel.Info) {
      return (
        level === LoggingLevel.Errors ||
        level === LoggingLevel.Warning ||
        level === LoggingLevel.Info
      );
    } else {
      return true;
    }
  }
}
