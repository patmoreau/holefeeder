import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import {
  FormGroup,
  FormGroupDirective,
  ReactiveFormsModule,
} from '@angular/forms';
import { DatePickerComponent } from '@app/shared/components';
import { AccountTypeNames } from '@app/shared/models';

@Component({
  selector: 'app-account-edit',
  templateUrl: './account-edit.component.html',
  styleUrls: ['./account-edit.component.scss'],
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, DatePickerComponent],
})
export class AccountEditComponent implements OnInit {
  form!: FormGroup;

  accountTypesNames = AccountTypeNames;

  constructor(private rootFormGroup: FormGroupDirective) {}

  ngOnInit(): void {
    this.form = this.rootFormGroup.control;
  }
}
