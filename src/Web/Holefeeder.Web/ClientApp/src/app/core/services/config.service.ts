import { Injectable } from '@angular/core';
import { StateService } from './state.service';
import { Observable } from 'rxjs';
import { LoggingLevel } from '@app/shared/models/logging-level.enum';

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
