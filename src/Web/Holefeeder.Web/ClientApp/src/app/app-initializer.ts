import {APP_INITIALIZER, FactoryProvider} from "@angular/core";
import {Observable, of} from "rxjs";


function loadConfigFactory(): () => Observable<void> {
  return () => of(console.log(`config initialized`));
}

export const loadConfigProvider: FactoryProvider = {
  provide: APP_INITIALIZER,
  useFactory: loadConfigFactory,
  deps: [],
  multi: true
};
