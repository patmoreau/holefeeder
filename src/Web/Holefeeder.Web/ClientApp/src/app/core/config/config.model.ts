import {Injectable} from "@angular/core";
import {Adapter} from "@app/shared/interfaces/adapter.interface";

export class Config {
  constructor(
    public apiUrl: string,
    public redirectUrl: string
  ) {
  }
}

@Injectable({providedIn: 'root'})
export class ConfigAdapter implements Adapter<Config> {
  adapt(item: any): Config {
    return new Config(item.apiUrl, item.redirectUrl);
  }
}
