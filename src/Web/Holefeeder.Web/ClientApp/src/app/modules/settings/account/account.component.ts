import {Component, OnInit} from '@angular/core';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {Router} from '@angular/router';
import {LoggerService} from "@app/core/logger/logger.service";

@Component({
  selector: 'app-account',
  templateUrl: './account.component.html',
  styleUrls: ['./account.component.scss']
})
export class AccountComponent implements OnInit {
  profileForm!: FormGroup;

  constructor(private formBuilder: FormBuilder, private router: Router, private logger: LoggerService) {
  }

  get firstName() {
    return this.profileForm.get('firstName');
  }

  get lastName() {
    return this.profileForm.get('lastName');
  }

  ngOnInit() {
    this.profileForm = this.formBuilder.group({
      firstName: ['Patrick', [Validators.required, Validators.pattern('[a-zA-Z].*')]],
      lastName: ['Moreau', [Validators.required]]
    });
  }

  saveProfile(formValues: any): void {
    this.logger.logVerbose(formValues);
  }

  cancel() {
    this.router.navigate(['/accounts']);
  }
}
