import { Component, OnInit } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { Router } from '@angular/router';
import { trace } from '@app/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-account',
  templateUrl: './account.component.html',
  styleUrls: ['./account.component.scss'],
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
})
export class AccountComponent implements OnInit {
  profileForm!: FormGroup;

  constructor(private formBuilder: FormBuilder, private router: Router) {}

  get firstName() {
    return this.profileForm.get('firstName');
  }

  get lastName() {
    return this.profileForm.get('lastName');
  }

  ngOnInit() {
    this.profileForm = this.formBuilder.group({
      firstName: [
        'Patrick',
        [Validators.required, Validators.pattern('[a-zA-Z].*')],
      ],
      lastName: ['Moreau', [Validators.required]],
    });
  }

  @trace()
  saveProfile(formValues: any): void {}

  cancel() {
    this.router.navigate(['/accounts']);
  }
}
