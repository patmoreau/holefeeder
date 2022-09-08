import { Location } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AccountType } from '@app/shared/models';
import { startOfToday } from 'date-fns';
import { OpenAccountAdapter } from '../models/open-account-command.model';
import { AccountCommandsService } from '../services/account-commands.service';

@Component({
  selector: 'app-open-account',
  templateUrl: './open-account.component.html',
  styleUrls: ['./open-account.component.scss'],
})
export class OpenAccountComponent implements OnInit {
  form!: FormGroup;

  constructor(
    private router: Router,
    private formBuilder: FormBuilder,
    private location: Location,
    private commandsService: AccountCommandsService,
    private adapter: OpenAccountAdapter
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
      .open(this.adapter.adapt(this.form.value))
      .subscribe(id => this.router.navigate(['accounts', id]));
  }

  goBack(): void {
    this.location.back();
  }
}
