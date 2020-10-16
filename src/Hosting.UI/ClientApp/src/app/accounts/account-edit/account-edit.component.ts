import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { IAccount } from '@app/shared/interfaces/account.interface';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { AccountsService } from '@app/shared/services/accounts.service';
import { Account } from '@app/shared/models/account.model';
import { AccountTypeNames } from '@app/shared/enums/account-type.enum';
import { NgbDateAdapter } from '@ng-bootstrap/ng-bootstrap';
import { NgbDateParserAdapter } from '@app/shared/ngb-date-parser.adapter';
import { faArrowLeft, faCalendarAlt } from '@fortawesome/free-solid-svg-icons';

@Component({
  selector: 'dfta-account-edit',
  templateUrl: './account-edit.component.html',
  styleUrls: ['./account-edit.component.scss'],
  providers: [{ provide: NgbDateAdapter, useClass: NgbDateParserAdapter }]
})
export class AccountEditComponent implements OnInit {
  isLoaded = false;
  account: IAccount;
  isDirty = true;
  faArrowLeft = faArrowLeft;
  faCalendarAlt = faCalendarAlt;

  accountForm: FormGroup;

  accountTypesNames = AccountTypeNames;

  constructor(
    private activatedRoute: ActivatedRoute,
    private accountsService: AccountsService,
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
    const id = this.activatedRoute.parent.snapshot.paramMap.get('accountId');
    if (id) {
      this.account = await this.accountsService.findOneById(id);
      this.accountForm.patchValue(this.account);
    } else {
      this.account = new Account();
    }
    this.isLoaded = true;
  }

  async onSubmit() {
    let accountId: string;
    if (this.account.id) {
      accountId = this.account.id;
      await this.accountsService.update(
        this.account.id,
        Object.assign(new Account(), this.accountForm.value, {
          id: this.account.id
        })
      );
    } else {
      const newAccount = await this.accountsService.create(
        Object.assign(new Account(), this.accountForm.value)
      );
      accountId = newAccount.id;
    }
    this.router.navigate(['accounts', accountId]);
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
