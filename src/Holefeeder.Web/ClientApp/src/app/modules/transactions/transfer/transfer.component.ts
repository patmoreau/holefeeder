import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import {
  FormControl,
  FormGroup,
  FormGroupDirective,
  ReactiveFormsModule,
} from '@angular/forms';
import { AccountsService } from '@app/core/services';
import {
  DatePickerComponent,
  DecimalInputComponent,
} from '@app/shared/components';
import { AccountInfo } from '@app/shared/models';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-transfer',
  templateUrl: './transfer.component.html',
  styleUrls: ['./transfer.component.scss'],
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    DatePickerComponent,
    DecimalInputComponent
  ]
})
export class TransferComponent implements OnInit {
  form!: FormGroup;

  values$!: Observable<AccountInfo[]>;

  constructor(
    private rootFormGroup: FormGroupDirective,
    private accountsService: AccountsService
  ) { }

  ngOnInit() {
    this.form = this.rootFormGroup.control;

    this.values$ = this.accountsService.activeAccounts$;
  }

  get amount(): FormControl {
    return this.form.get('amount') as FormControl;
  }
}
