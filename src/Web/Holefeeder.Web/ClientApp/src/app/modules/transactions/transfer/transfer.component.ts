import { Component, OnInit } from '@angular/core';
import { FormGroup, FormGroupDirective } from '@angular/forms';
import { AccountInfo } from '@app/core/models';
import { AccountsService } from '@app/core/services';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-transfer',
  templateUrl: './transfer.component.html',
  styleUrls: ['./transfer.component.scss'],
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
