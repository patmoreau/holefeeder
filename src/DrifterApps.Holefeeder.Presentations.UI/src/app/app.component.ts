import { Component } from '@angular/core';
import { OAuthService, JwksValidationHandler } from 'angular-oauth2-oidc';
import { googleAuthConfig } from './google-auth.config';

@Component({
  selector: 'dfta-root',
  templateUrl: './app.component.html',
})
export class AppComponent {

  constructor(private oauthService: OAuthService) {
    this.configureImplicitFlow();
  }

  private configureImplicitFlow() {
    this.oauthService.configure(googleAuthConfig);
    this.oauthService.tokenValidationHandler = new JwksValidationHandler();
    this.oauthService.loadDiscoveryDocumentAndTryLogin();
    this.oauthService.setupAutomaticSilentRefresh();
  }
}
