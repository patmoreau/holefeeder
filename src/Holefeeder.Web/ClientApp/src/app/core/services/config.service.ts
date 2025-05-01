import { Injectable } from '@angular/core';
import { StateService } from './state.service';
import { Observable, distinctUntilChanged } from 'rxjs';
import { LoggingLevel } from '@app/shared/models/logging-level.enum';

interface ConfigState {
  loggingLevel: LoggingLevel;
  lastUpdate: number;
}

const initialState: ConfigState = {
  loggingLevel: LoggingLevel.Info,
  lastUpdate: 0
};

@Injectable({ providedIn: 'root' })
export class ConfigService extends StateService<ConfigState> {
  loggingLevel$: Observable<LoggingLevel> = this.select(
    state => state.loggingLevel
  ).pipe(distinctUntilChanged());

  constructor() {
    super(initialState);
  }

  setLoggingLevel(loggingLevel: LoggingLevel) {
    this.setState({
      loggingLevel: loggingLevel,
      lastUpdate: Date.now()
    });
  }
}
