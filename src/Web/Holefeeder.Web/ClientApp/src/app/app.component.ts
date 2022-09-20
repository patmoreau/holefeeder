import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { RouterModule } from '@angular/router';
import { OidcSecurityService } from 'angular-auth-oidc-client';
import { loadConfigProvider } from './app-initializer';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  standalone: true,
  imports: [CommonModule, RouterModule],
  providers: [loadConfigProvider],
})
export class AppComponent {
  constructor(public oidcSecurityService: OidcSecurityService) {}
}
