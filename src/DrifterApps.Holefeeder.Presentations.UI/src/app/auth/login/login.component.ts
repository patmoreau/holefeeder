import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormBuilder, FormGroup } from '@angular/forms';
import { OAuthService } from 'angular-oauth2-oidc';


// TODO: http://jasonwatmore.com/post/2018/10/29/angular-7-user-registration-and-login-example-tutorial
@Component({
  selector: 'dfta-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {
  loginForm: FormGroup;

  constructor(private oauthService: OAuthService, private formBuilder: FormBuilder) {
    this.loginForm = this.formBuilder.group({
      username: [],
      password: [],
    });
  }

  ngOnInit(): void {
  }

  onSubmit() {
    this.oauthService.initLoginFlow();
  }
}
