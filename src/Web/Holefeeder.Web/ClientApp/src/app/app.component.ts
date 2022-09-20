import { CommonModule } from '@angular/common';
import { Component, ErrorHandler } from '@angular/core';
import { RouterModule } from '@angular/router';
import { OidcSecurityService } from 'angular-auth-oidc-client';
import { loadConfigProvider } from './app-initializer';
import { environment } from '@env/environment';
import { GlobalErrorHandler } from '@app/core/errors/global-error-handler';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { HttpLoadingInterceptor } from '@app/core/errors/http-loading.interceptor';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  standalone: true,
  imports: [CommonModule, RouterModule],
})
export class AppComponent {
  constructor(public oidcSecurityService: OidcSecurityService) {}
}
