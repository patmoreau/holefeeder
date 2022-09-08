import { environment } from '@env/environment';
import { enableProdMode } from '@angular/core';
import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';
import { AppModule } from '@app/app.module';
import { MsalGuard, MsalInterceptor, MsalService } from '@azure/msal-angular';
import { of } from 'rxjs';

MsalGuard.prototype.canActivate = () => of(true);

MsalInterceptor.prototype.intercept = (req, next) => {
  const access = localStorage.getItem('access_token');
  req = req.clone({
    setHeaders: {
      Authorization: `Bearer ${access}`,
    },
  });
  return next.handle(req);
};

// MsalService.prototype.instance.getActiveAccount = (): any => {
//   if (!localStorage.getItem('access_token')) return undefined;
//   return {
//     idToken: {
//       scope: [],
//       // other claims if required
//     },
//   };
// };

if (environment.production) {
  enableProdMode();
}

platformBrowserDynamic()
  .bootstrapModule(AppModule)
  .catch((err) => console.error(err));
