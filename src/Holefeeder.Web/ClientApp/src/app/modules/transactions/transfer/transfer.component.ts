import { CommonModule } from '@angular/common';
import { Component, OnInit, inject } from '@angular/core';
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
  private rootFormGroup = inject(FormGroupDirective);
  private accountsService = inject(AccountsService);

  form!: FormGroup;

  values$!: Observable<AccountInfo[]>;

  ngOnInit() {
    this.form = this.rootFormGroup.control;

    this.values$ = this.accountsService.activeAccounts$;
  }

  get amount(): FormControl {
    return this.form.get('amount') as FormControl;
  }
}
