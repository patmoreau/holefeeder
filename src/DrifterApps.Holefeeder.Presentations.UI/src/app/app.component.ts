import { Component, OnInit } from '@angular/core';
import { OidcSecurityService } from 'angular-auth-oidc-client';

@Component({
  selector: 'dfta-root',
  templateUrl: './app.component.html',
})
export class AppComponent implements OnInit {
  constructor(public oidcSecurityService: OidcSecurityService) {}

  ngOnInit() {
      this.oidcSecurityService.checkAuth().subscribe((auth) => console.log('is authenticated', auth));
  }
}
