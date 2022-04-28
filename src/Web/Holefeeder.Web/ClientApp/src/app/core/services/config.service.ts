import {Injectable} from "@angular/core";
import {StateService} from "@app/core/services/state.service";
import {Observable} from "rxjs";

export class LoggingLevel {
  public static None = 'None';
  public static Verbose = 'Verbose';
  public static Info = 'Info';
  public static Warning = 'Warning';
  public static Errors = 'Errors';
}

interface ConfigState {
  loggingLevel: LoggingLevel;
}

const initialState: ConfigState = {
  loggingLevel: LoggingLevel.None
};

@Injectable({providedIn: 'root'})
export class ConfigService extends StateService<ConfigState> {

  loggingLevel$: Observable<LoggingLevel> = this.select((state) => state.loggingLevel);

  constructor() {
    super(initialState);
  }

  setLoggingLevel(loggingLevel: LoggingLevel) {
    this.setState({loggingLevel: loggingLevel});
  }
}
