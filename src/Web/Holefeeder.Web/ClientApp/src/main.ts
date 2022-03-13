import {enableProdMode} from '@angular/core';
import {platformBrowserDynamic} from '@angular/platform-browser-dynamic';

import {environment} from '@env/environment';
import {AppServerModule} from "@app/app.server.module";
import {AppModule} from "@app/app.module";

// export function getBaseUrl() {
//   return document.getElementsByTagName('base')[0].href;
// }
//
// const providers = [
//   {provide: 'BASE_URL', useFactory: getBaseUrl, deps: []}
// ];

if (environment.production) {
  enableProdMode();
}

platformBrowserDynamic().bootstrapModule(AppModule)
  .catch(err => console.error(err));
// platformBrowserDynamic().bootstrapModule(AppServerModule)
//   .catch(err => console.error(err));
