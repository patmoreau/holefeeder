import { Injectable } from '@angular/core';
import { LoggingLevel } from '@app/core/logger/logging-level.enum';
import { StateService } from './state.service';
import { Observable } from 'rxjs';

interface ConfigState {
  loggingLevel: LoggingLevel;
}

const initialState: ConfigState = {
  loggingLevel: LoggingLevel.Info,
};

@Injectable({ providedIn: 'root' })
export class ConfigService extends StateService<ConfigState> {
  loggingLevel$: Observable<LoggingLevel> = this.select(
    state => state.loggingLevel
  );

  constructor() {
    super(initialState);
  }

  setLoggingLevel(loggingLevel: LoggingLevel) {
    this.setState({ loggingLevel: loggingLevel });
  }
}
