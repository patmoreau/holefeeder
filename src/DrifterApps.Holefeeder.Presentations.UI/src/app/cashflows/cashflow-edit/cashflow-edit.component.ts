import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { Location } from '@angular/common';
import { FormBuilder, FormGroup, Validators, FormArray } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { CashflowsService } from '@app/shared/services/cashflows.service';
import { NgbDateAdapter, NgbModalRef } from '@ng-bootstrap/ng-bootstrap';
import { AccountsService } from '@app/shared/services/accounts.service';
import { ICategory } from '@app/shared/interfaces/category.interface';
import { CategoriesService } from '@app/shared/services/categories.service';
import { IAccountDetail } from '@app/shared/interfaces/account-detail.interface';
import { NgbDateParserAdapter } from '@app/shared/ngb-date-parser.adapter';
import { DateIntervalTypeNames, DateIntervalType } from '@app/shared/enums/date-interval-type.enum';
import { ICashflow } from '@app/shared/interfaces/cashflow.interface';
import { Cashflow } from '@app/shared/models/cashflow.model';
import { faArrowLeft, faCalendarAlt } from '@fortawesome/free-solid-svg-icons';
import { startOfToday } from 'date-fns';

@Component({
  selector: 'dfta-cashflow-edit',
  templateUrl: './cashflow-edit.component.html',
  styleUrls: ['./cashflow-edit.component.scss'],
  providers: [{ provide: NgbDateAdapter, useClass: NgbDateParserAdapter }]
})
export class CashflowEditComponent implements OnInit {
  @ViewChild('confirm', { static: true })
  confirmModalElement: ElementRef;
  confirmModal: NgbModalRef;
  confirmMessages: string;

  account: string;
  cashflow: ICashflow;
  cashflowForm: FormGroup;

  accounts: IAccountDetail[];
  categories: ICategory[];

  isLoaded = false;

  intervalTypesNames = DateIntervalTypeNames;

  faArrowLeft = faArrowLeft;
  faCalendarAlt = faCalendarAlt;

  constructor(
    private activatedRoute: ActivatedRoute,
    private accountsService: AccountsService,
    private cashflowsService: CashflowsService,
    private categoriesService: CategoriesService,
    private formBuilder: FormBuilder,
    private location: Location
  ) {
    this.cashflowForm = this.formBuilder.group({
      amount: ['', [Validators.required, Validators.min(0)]],
      intervalType: [],
      frequency: [],
      effectiveDate: [],
      account: ['', [Validators.required]],
      category: ['', [Validators.required]],
      description: ['', [Validators.required]],
      inactive: [],
      tags: this.formBuilder.array([])
    });
  }

  async ngOnInit() {

    this.account = this.activatedRoute.snapshot.queryParamMap.get('account');

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

    if (this.activatedRoute.snapshot.paramMap.has('cashflowId')) {
      this.cashflow = await this.cashflowsService.findOneById(
        this.activatedRoute.snapshot.paramMap.get('cashflowId')
      );
    } else {
      this.cashflow = Object.assign(new Cashflow(), {
        account: this.account ? this.account : this.accounts[0].id,
        category: this.categories[0].id,
        intervalType: DateIntervalType.monthly,
        effectiveDate: startOfToday(),
        inactive: false
      });
    }
    this.cashflowForm.patchValue(this.cashflow);
    if (this.cashflow.tags) {
      const tags = this.cashflowForm.get('tags') as FormArray;
      this.cashflow.tags.forEach(t => tags.push(this.formBuilder.control(t)));
    }

    this.isLoaded = true;
  }

  goBack(): void {
    this.location.back();
  }

  onSubmit() {
    const cashflow = Object.assign(
      new Cashflow(),
      this.cashflow,
      this.cashflowForm.value
    );
    if (this.cashflow.id) {
      this.cashflowsService.update(this.cashflow.id, cashflow);
    } else {
      this.cashflowsService.create(cashflow);
    }

    this.location.back();
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
