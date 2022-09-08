import { Location } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import {
  MakePurchaseCommandAdapter,
  TransferMoneyCommandAdapter,
} from '@app/core/models';
import {
  AccountsService,
  CategoriesService,
  TransactionsService,
} from '@app/core/services';
import { DateIntervalType } from '@app/shared/models';
import { startOfToday } from 'date-fns';
import { combineLatest, filter, Observable, tap } from 'rxjs';

const accountIdParamName = 'accountId';

@Component({
  selector: 'app-make-purchase',
  templateUrl: './make-purchase.component.html',
  styleUrls: ['./make-purchase.component.scss'],
})
export class MakePurchaseComponent implements OnInit {
  public isNotRecurring = true;

  formPurchase!: FormGroup;
  formTransfer!: FormGroup;

  values$!: Observable<[any, any]>;

  constructor(
    private route: ActivatedRoute,
    private formBuilder: FormBuilder,
    private location: Location,
    private accountService: AccountsService,
    private categoriesService: CategoriesService,
    private transactionsService: TransactionsService,
    private adapterPurchase: MakePurchaseCommandAdapter,
    private adapterTransfer: TransferMoneyCommandAdapter
  ) {}

  ngOnInit(): void {
    this.formPurchase = this.formBuilder.group({
      amount: ['', [Validators.required, Validators.min(0)]],
      date: ['', [Validators.required]],
      account: ['', [Validators.required]],
      category: ['', [Validators.required]],
      description: [''],
      tags: this.formBuilder.array([]),
      isRecurring: [''],
      cashflow: this.formBuilder.group({
        intervalType: [''],
        effectiveDate: [''],
        frequency: [''],
      }),
    });

    this.formTransfer = this.formBuilder.group({
      amount: ['', [Validators.required, Validators.min(0)]],
      date: ['', [Validators.required]],
      fromAccount: ['', [Validators.required]],
      toAccount: ['', [Validators.required]],
      description: [''],
    });

    this.values$ = combineLatest([
      this.route.params,
      this.accountService.activeAccounts$,
    ]).pipe(
      filter(([_, accounts]) => accounts.length > 0),
      tap(([params, accounts]) => {
        let fromAccount =
          params[accountIdParamName] ??
          this.accountService.findOneByIndex(0)?.id;
        let toAccount = this.accountService.findOneByIndex(1)?.id;
        this.formPurchase.patchValue({
          date: startOfToday(),
          account: fromAccount,
          category: this.categoriesService.findOneByIndex(0)?.id,
          isRecurring: false,
          cashflow: {
            intervalType: DateIntervalType.monthly,
            frequency: 1,
            effectiveDate: startOfToday(),
          },
        });
        this.formTransfer.patchValue({
          date: startOfToday(),
          fromAccount: fromAccount,
          toAccount: toAccount,
        });
      })
    );
  }

  onMakePurchase(): void {
    this.transactionsService
      .makePurchase(
        this.adapterPurchase.adapt(
          Object.assign({}, this.formPurchase.value, {
            cashflow:
              this.formPurchase.get('isRecurring')!.value === true
                ? this.formPurchase.get('cashflow')?.value
                : null,
          })
        )
      )
      .subscribe(_ => this.location.back());
  }

  onTransfer(): void {
    this.transactionsService
      .transfer(this.adapterTransfer.adapt(this.formTransfer.value))
      .subscribe(_ => this.location.back());
  }

  goBack(): void {
    this.location.back();
  }
}
