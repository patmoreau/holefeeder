import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { AccountTypeNames } from '@app/shared/enums/account-type.enum';
import { NgbDateAdapter } from '@ng-bootstrap/ng-bootstrap';
import { NgbDateParserAdapter } from '@app/shared/ngb-date-parser.adapter';
import { Observable, tap } from 'rxjs';
import { Account, AccountAdapter } from '../models/account.model';
import { AccountsService } from '../services/accounts.service';

@Component({
  selector: 'dfta-account-edit',
  templateUrl: './account-edit.component.html',
  styleUrls: ['./account-edit.component.scss'],
  providers: [{ provide: NgbDateAdapter, useClass: NgbDateParserAdapter }]
})
export class AccountEditComponent implements OnInit {
  account$: Observable<Account> | undefined;
  isDirty = true;

  accountForm: FormGroup;

  accountTypesNames = AccountTypeNames;

  constructor(
    private activatedRoute: ActivatedRoute,
    private accountsService: AccountsService,
    private adapter: AccountAdapter,
    private formBuilder: FormBuilder,
    private router: Router
  ) {
    this.accountForm = this.formBuilder.group({
      name: [null, Validators.required],
      type: [null, Validators.required],
      openBalance: [0, Validators.required],
      openDate: [null, Validators.required],
      description: [],
      favorite: [],
      inactive: [],
    });
  }

  async ngOnInit() {
    this.activatedRoute.params.subscribe(async params => {
      this.account$ = this.accountsService.findById(params['accountId'])
        .pipe(
          tap(account => this.accountForm.patchValue(account))
        );
    });
  }

  async onSubmit() {
    this.accountsService.save(this.adapter.adapt(this.accountForm.value))
      .subscribe(id => this.router.navigate(['accounts', id]));
  }

  compareFn(optionOne: any, optionTwo: any): boolean {
    if (optionOne && optionTwo) {
      const idOne = typeof optionOne === 'string' ? optionOne : optionOne.id;
      const idTwo = typeof optionTwo === 'string' ? optionTwo : optionTwo.id;
      return idOne === idTwo;
    }
    return false;
  }
}
