import { Injectable } from '@angular/core';
import { LoggingLevel } from '@app/core/logger';
import { StateService } from './state.service';

interface ConfigState {
  loggingLevel: LoggingLevel;
}

const initialState: ConfigState = {
  loggingLevel: LoggingLevel.None,
};

@Injectable({ providedIn: 'root' })
export class ConfigService extends StateService<ConfigState> {
  constructor() {
    super(initialState);
  }

  setLoggingLevel(loggingLevel: LoggingLevel) {
    this.setState({ loggingLevel: loggingLevel });
  }
}
