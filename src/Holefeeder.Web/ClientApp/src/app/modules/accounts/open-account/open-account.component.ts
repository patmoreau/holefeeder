import { CommonModule, Location } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { Router } from '@angular/router';
import { AccountType } from '@app/shared/models';
import { startOfToday } from 'date-fns';
import { OpenAccountCommand } from '../models/open-account-command.model';
import { AccountCommandsService } from '../services/account-commands.service';
import { AccountEditComponent } from '@app/modules/accounts/account-edit/account-edit.component';

@Component({
    selector: 'app-open-account',
    templateUrl: './open-account.component.html',
    styleUrls: ['./open-account.component.scss'],
    imports: [CommonModule, ReactiveFormsModule, AccountEditComponent]
})
export class OpenAccountComponent implements OnInit {
  form!: FormGroup;

  constructor(
    private router: Router,
    private formBuilder: FormBuilder,
    private location: Location,
    private commandsService: AccountCommandsService
  ) {}

  ngOnInit(): void {
    this.form = this.formBuilder.group({
      name: ['', Validators.required],
      type: ['', Validators.required],
      openBalance: [0, [Validators.required, Validators.min(0)]],
      openDate: ['', Validators.required],
      description: [''],
    });

    this.form.patchValue({
      type: AccountType.checking,
      openDate: startOfToday(),
    });
  }

  onSubmit(): void {
    this.commandsService
      .open(OpenAccountCommand.fromObject(this.form.value))
      .subscribe(id => this.router.navigate(['accounts', id]));
  }

  goBack(): void {
    this.location.back();
  }
}
