import { CommonModule } from '@angular/common';
import { Component, OnInit, inject } from '@angular/core';
import {
  FormControl,
  FormGroup,
  FormGroupDirective,
  ReactiveFormsModule,
} from '@angular/forms';
import {
  DatePickerComponent,
  DecimalInputComponent,
} from '@app/shared/components';
import { AccountTypeNames } from '@app/shared/models';

@Component({
  selector: 'app-account-edit',
  templateUrl: './account-edit.component.html',
  styleUrls: ['./account-edit.component.scss'],
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    DatePickerComponent,
    DecimalInputComponent,
  ]
})
export class AccountEditComponent implements OnInit {
  private rootFormGroup = inject(FormGroupDirective);

  form!: FormGroup;

  accountTypesNames = AccountTypeNames;

  ngOnInit(): void {
    this.form = this.rootFormGroup.control;
  }

  get openBalance(): FormControl {
    return this.form.get('openBalance') as FormControl;
  }
}
