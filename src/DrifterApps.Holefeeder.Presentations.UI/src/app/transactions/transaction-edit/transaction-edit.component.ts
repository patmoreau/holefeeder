import { Component, OnInit, ViewChild, ElementRef, TemplateRef } from '@angular/core';
import { Location } from '@angular/common';
import { FormBuilder, FormGroup, Validators, FormArray } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { CashflowsService } from '@app/shared/services/cashflows.service';
import { NgbDateAdapter, NgbModalRef, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ITransaction } from '@app/shared/interfaces/transaction.interface';
import { Transaction } from '@app/shared/models/transaction.model';
import { AccountsService } from '@app/shared/services/accounts.service';
import { ICategory } from '@app/shared/interfaces/category.interface';
import { CategoriesService } from '@app/shared/services/categories.service';
import { TransactionsService } from '@app/shared/services/transactions.service';
import { IAccountDetail } from '@app/shared/interfaces/account-detail.interface';
import { NgbDateParserAdapter } from '@app/shared/ngb-date-parser.adapter';
import { startOfToday } from 'date-fns';
import { faCalendarAlt } from '@fortawesome/free-solid-svg-icons';

@Component({
  selector: 'dfta-transaction-edit',
  templateUrl: './transaction-edit.component.html',
  styleUrls: ['./transaction-edit.component.scss'],
  providers: [{ provide: NgbDateAdapter, useClass: NgbDateParserAdapter }]
})
export class TransactionEditComponent implements OnInit {
  @ViewChild('confirm', { static: true })
  confirmModalElement: ElementRef;
  confirmModal: NgbModalRef;
  confirmMessages: string;

  account: string;
  transaction: ITransaction;
  transactionForm: FormGroup;

  accounts: IAccountDetail[];
  categories: ICategory[];

  isLoaded = false;
  isNew = false;

  faCalendarAlt = faCalendarAlt;

  constructor(
    private route: ActivatedRoute,
    private accountsService: AccountsService,
    private cashflowsService: CashflowsService,
    private categoriesService: CategoriesService,
    private transactionsService: TransactionsService,
    private modalService: NgbModal,
    private formBuilder: FormBuilder,
    private location: Location
  ) {
    this.transactionForm = this.formBuilder.group({
      amount: ['', [Validators.required, Validators.min(0)]],
      date: [''],
      account: ['', [Validators.required]],
      category: ['', [Validators.required]],
      description: [''],
      tags: this.formBuilder.array([])
    });
  }

  async ngOnInit() {
    this.accounts = await this.accountsService.find(null, null, [
      '-favorite',
      'name'
    ], [
      'inactive!=true'
    ]);

    this.categories = await this.categoriesService.find(null, null, [
      '-favorite',
      'name'
    ]);

    this.account = this.route.parent.snapshot.queryParamMap.get('accountId');

    if (this.route.snapshot.paramMap.has('transactionId')) {
      const transactionId = this.route.snapshot.paramMap.get('transactionId');
      this.transaction = await this.transactionsService.findOneById(transactionId);
    } else {
      this.isNew = true;
      const date = this.route.snapshot.queryParamMap.has('date') ? new Date(this.route.snapshot.queryParamMap.get('date')) : startOfToday();
      if (this.route.snapshot.queryParamMap.has('cashflow')) {
        const cashflowId = this.route.snapshot.queryParamMap.get('cashflow');
        const cashflow = await this.cashflowsService.findOneById(cashflowId);

        this.transaction = Object.assign(new Transaction(), {
          account: cashflow.account,
          cashflow: cashflow.id,
          category: cashflow.category,
          amount: cashflow.amount,
          id: undefined,
          date: date,
          cashflowDate: date,
          description: cashflow.description,
          tags: cashflow.tags ? cashflow.tags : []
        });
      } else {
        this.transaction = Object.assign(new Transaction(), {
          account: this.account ? this.account : this.accounts[0].id,
          date: date,
          category: this.categories[0].id
        });
      }
    }

    this.transactionForm.patchValue(this.transaction);
    if (this.transaction.tags) {
      const tags = this.transactionForm.get('tags') as FormArray;
      this.transaction.tags.forEach(t => tags.push(this.formBuilder.control(t)));
    }
    this.isLoaded = true;
  }

  goBack(): void {
    this.location.back();
  }

  onSubmit() {
    const transaction = Object.assign(
      new Transaction(),
      this.transaction,
      this.transactionForm.value
    );
    if (this.transaction.id) {
      this.transactionsService.update(this.transaction.id, transaction);
    } else {
      this.transactionsService.create(transaction);
    }

    this.location.back();
  }

  onDelete(content: any) {
    this.modalService.open(content, { ariaLabelledBy: 'modal-delete-title' }).result.then(async (_) => {
      await this.transactionsService.delete(this.transaction.id);
      this.location.back();
    }, (_) => { });
  }

  onConfirm() { }

  compareFn(optionOne: any, optionTwo: any): boolean {
    if (optionOne && optionTwo) {
      const idOne = typeof optionOne === 'string' ? optionOne : optionOne.id;
      const idTwo = typeof optionTwo === 'string' ? optionTwo : optionTwo.id;
      return idOne === idTwo;
    }
    return false;
  }
}
