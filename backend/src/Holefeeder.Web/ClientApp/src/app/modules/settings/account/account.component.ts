
import { Component, OnInit, inject } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { Router } from '@angular/router';

@Component({
  selector: 'app-account',
  templateUrl: './account.component.html',
  styleUrls: ['./account.component.scss'],
  standalone: true,
  imports: [ReactiveFormsModule]
})
export class AccountComponent implements OnInit {
  private formBuilder = inject(FormBuilder);
  private router = inject(Router);

  profileForm!: FormGroup;

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

  // eslint-disable-next-line @typescript-eslint/no-empty-function,@typescript-eslint/no-unused-vars
  saveProfile(formValues: unknown): void { }

  cancel() {
    this.router.navigate(['/accounts']);
  }
}
