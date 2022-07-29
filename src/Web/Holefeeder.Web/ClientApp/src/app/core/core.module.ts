import {CommonModule} from '@angular/common';
import {HTTP_INTERCEPTORS} from '@angular/common/http';
import {ErrorHandler, NgModule} from '@angular/core';
import {GlobalErrorHandler} from './errors/global-error-handler';
import {HttpLoadingInterceptor} from './errors/http-loading.interceptor';
import {NgbModule} from "@ng-bootstrap/ng-bootstrap";
import {SharedModule} from "@app/shared/shared.module";

@NgModule({
  declarations: [],
  imports: [CommonModule, SharedModule, NgbModule],
  providers: [
    {
      provide: ErrorHandler,
      useClass: GlobalErrorHandler,
    },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: HttpLoadingInterceptor,
      multi: true,
    },
  ],
})
export class CoreModule {
}
