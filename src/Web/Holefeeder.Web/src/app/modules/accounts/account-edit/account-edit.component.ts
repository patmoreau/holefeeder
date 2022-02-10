import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormGroup, FormBuilder, Validators, FormGroupDirective } from '@angular/forms';
import { AccountTypeNames } from '@app/shared/enums/account-type.enum';
import { NgbDateAdapter } from '@ng-bootstrap/ng-bootstrap';
import { NgbDateParserAdapter } from '@app/shared/ngb-date-parser.adapter';
import { Observable, tap } from 'rxjs';
import { Account, AccountAdapter } from '../models/account.model';
import { AccountsService } from '../services/accounts.service';

@Component({
  selector: 'app-account-edit',
  templateUrl: './account-edit.component.html',
  styleUrls: ['./account-edit.component.scss'],
  providers: [{ provide: NgbDateAdapter, useClass: NgbDateParserAdapter }]
})
export class AccountEditComponent implements OnInit {

  form!: FormGroup;

  accountTypesNames = AccountTypeNames;

  constructor(
    private rootFormGroup: FormGroupDirective,
  ) {  }

  ngOnInit(): void {
    this.form = this.rootFormGroup.control;
  }
}
