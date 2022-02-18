import {Component, OnInit} from '@angular/core';
import {FormGroup, FormGroupDirective} from '@angular/forms';
import {Observable} from 'rxjs';
import {AccountsInfoService} from '@app/core/services/account-info.service';
import {NgbDateAdapter} from "@ng-bootstrap/ng-bootstrap";
import {NgbDateParserAdapter} from "@app/shared/ngb-date-parser.adapter";
import {AccountInfo} from "@app/core/models/account-info.model";

@Component({
  selector: 'app-transfer',
  templateUrl: './transfer.component.html',
  styleUrls: ['./transfer.component.scss'],
  providers: [{provide: NgbDateAdapter, useClass: NgbDateParserAdapter}]
})
export class TransferComponent implements OnInit {

  form!: FormGroup;

  values$!: Observable<AccountInfo[]>;

  constructor(
    private rootFormGroup: FormGroupDirective,
    private accountsService: AccountsInfoService
  ) {
  }

  ngOnInit() {
    this.form = this.rootFormGroup.control;

    this.values$ = this.accountsService.accounts$;
  }
}