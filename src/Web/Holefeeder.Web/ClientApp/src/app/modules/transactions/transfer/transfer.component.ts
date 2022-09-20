import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import {
  FormGroup,
  FormGroupDirective,
  ReactiveFormsModule,
} from '@angular/forms';
import { AccountInfo } from '@app/core/models';
import { AccountsService } from '@app/core/services';
import { DatePickerComponent } from '@app/shared/components';
import { AutofocusDirective } from '@app/shared/directives';
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
    AutofocusDirective,
  ],
})
export class TransferComponent implements OnInit {
  form!: FormGroup;

  values$!: Observable<AccountInfo[]>;

  constructor(
    private rootFormGroup: FormGroupDirective,
    private accountsService: AccountsService
  ) {}

  ngOnInit() {
    this.form = this.rootFormGroup.control;

    this.values$ = this.accountsService.activeAccounts$;
  }
}
