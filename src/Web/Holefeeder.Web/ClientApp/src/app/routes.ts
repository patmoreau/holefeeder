import { Routes } from '@angular/router';
import { ErrorHandler } from '@angular/core';
import { GlobalErrorHandler } from '@app/core/errors/global-error-handler';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { HttpLoadingInterceptor } from '@app/core/errors/http-loading.interceptor';
import { environment } from '@env/environment';

export const ROUTES: Routes = [
  {
    path: '',
    loadChildren: () =>
      import('./modules/home/routes').then(m => m.HOME_ROUTES),
    providers: [],
  },
];
