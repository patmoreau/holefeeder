import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { Router } from '@angular/router';

@Component({
  selector: 'dfta-account',
  templateUrl: './account.component.html',
  styleUrls: ['./account.component.scss']
})
export class AccountComponent implements OnInit {
  profileForm: FormGroup;

  constructor(private formBuilder: FormBuilder, private router: Router) {}

  ngOnInit() {
    this.profileForm = this.formBuilder.group({
      firstName: ['Patrick', [Validators.required, Validators.pattern('[a-zA-Z].*')]],
      lastName: ['Moreau', [Validators.required]]
    });
  }

  saveProfile(formValues) {
    console.debug(formValues);
  }

  cancel() {
    this.router.navigate(['/accounts']);
  }

  get firstName() {
    return this.profileForm.get('firstName');
  }

  get lastName() {
    return this.profileForm.get('lastName');
  }
}
