import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { faGoogle } from '@fortawesome/free-brands-svg-icons';
import { AuthenticationService } from '../services/authentication.service';
import { OidcSecurityService } from 'angular-auth-oidc-client';
import { Router } from '@angular/router';

@Component({
  selector: 'dfta-login',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss']
})
export class RegisterComponent implements OnInit {
  registerForm: FormGroup;
  loading = false;
  faGoogle = faGoogle;

  constructor(
    private authService: AuthenticationService,
    private oidcSecurityService: OidcSecurityService,
    private router: Router,
    private formBuilder: FormBuilder) {
    this.registerForm = this.formBuilder.group({
      username: [],
      password: [],
    });
  }

  async ngOnInit(): Promise<void> {
    const isAuth = await this.oidcSecurityService.isAuthenticated$.toPromise();
    console.log('DrifterApps:registered', isAuth);
    if (isAuth) {
      this.router.navigate(['/auth/register']);
    }
  }

  onSubmit() {
    this.authService.login();
  }
}
