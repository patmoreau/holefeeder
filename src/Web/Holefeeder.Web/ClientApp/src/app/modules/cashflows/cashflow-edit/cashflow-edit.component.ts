import {Component, ElementRef, OnInit, ViewChild} from '@angular/core';
import {Location} from '@angular/common';
import {FormArray, FormBuilder, FormGroup, Validators} from '@angular/forms';
import {ActivatedRoute} from '@angular/router';
import {NgbDateAdapter, NgbModalRef} from '@ng-bootstrap/ng-bootstrap';
import {NgbDateParserAdapter} from '@app/shared/ngb-date-parser.adapter';
import {DateIntervalTypeNames} from '@app/shared/enums/date-interval-type.enum';
import {Observable} from 'rxjs';
import {CategoriesService} from '@app/core/services/categories.service';
import {Category} from '@app/core/models/category.model';
import {CashflowDetail} from '@app/core/models/cashflow-detail.model';
import {CashflowsService} from '@app/core/services/cashflows.service';
import {Account} from '@app/core/models/account.model';
import {AccountsService} from '@app/core/services/accounts.service';

@Component({
  selector: 'app-cashflow-edit',
  templateUrl: './cashflow-edit.component.html',
  styleUrls: ['./cashflow-edit.component.scss'],
  providers: [{provide: NgbDateAdapter, useClass: NgbDateParserAdapter}]
})
export class CashflowEditComponent implements OnInit {
  @ViewChild('confirm', {static: true})
  confirmModalElement!: ElementRef;
  confirmModal!: NgbModalRef;
  confirmMessages!: string;

  account!: string;
  cashflow!: CashflowDetail;
  cashflowForm: FormGroup;

  accounts$!: Observable<Account[]>;
  categories$!: Observable<Category[]>;

  isLoaded = false;

  intervalTypesNames = DateIntervalTypeNames;

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

  get tags(): FormArray {
    return this.cashflowForm.get('tags') as FormArray
  }

  async ngOnInit() {

    this.account = this.activatedRoute.snapshot.queryParamMap.get('account')!;

    this.accounts$ = this.accountsService.activeAccounts$;
    this.categories$ = this.categoriesService.categories$;

    if (this.activatedRoute.snapshot.paramMap.has('cashflowId')) {
      // this.cashflow = await this.cashflowsService.findOneById(
      //   this.activatedRoute.snapshot.paramMap.get('cashflowId')
      // );
    } else {
      // this.cashflow = Object.assign(new CashflowDetail(), {
      //   account: this.account ? this.account : this.accountsService.findOneByIndex(0).id,
      //   // category: this.categories[0].id,
      //   intervalType: DateIntervalType.monthly,
      //   effectiveDate: startOfToday(),
      // });
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
    // const cashflow = Object.assign(
    //   new Cashflow(),
    //   this.cashflow,
    //   this.cashflowForm.value
    // );
    // if (this.cashflow.id) {
    //   this.cashflowsService.update(this.cashflow.id, cashflow);
    // } else {
    //   this.cashflowsService.create(cashflow);
    // }

    this.location.back();
  }

  onConfirm() {
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
