import { CommonModule, Location } from '@angular/common';
import { Component, inject, OnInit } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import {
  MakePurchaseCommandAdapter,
  TransferMoneyCommandAdapter,
} from '@app/core/adapters';
import { AccountsService, TransactionsService } from '@app/core/services';
import { TransactionEditComponent } from '@app/modules/transactions/transaction-edit/transaction-edit.component';
import { TransferComponent } from '@app/modules/transactions/transfer/transfer.component';
import {
  LoaderComponent,
  RecurringCashflowComponent,
} from '@app/shared/components';
import { DateIntervalType } from '@app/shared/models';
import { NgbCollapseModule } from '@ng-bootstrap/ng-bootstrap';
import { startOfToday } from 'date-fns';
import { combineLatest, filter, Observable, tap } from 'rxjs';
import { Store } from '@ngrx/store';
import { AppState } from '@app/core/store';
import { filterNullish } from '@app/shared/helpers';
import { CategoriesFeature } from '@app/core/store/categories';

const accountIdParamName = 'accountId';

@Component({
  selector: 'app-make-purchase',
  templateUrl: './make-purchase.component.html',
  styleUrls: ['./make-purchase.component.scss'],
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    NgbCollapseModule,
    TransactionEditComponent,
    TransferComponent,
    RecurringCashflowComponent,
    LoaderComponent,
  ]
})
export class MakePurchaseComponent implements OnInit {
  public isNotRecurring = true;

  formPurchase!: FormGroup;
  formTransfer!: FormGroup;

  values$!: Observable<[unknown, unknown]>;

  private readonly store: Store<AppState> = inject(Store<AppState>);

  constructor(
    private route: ActivatedRoute,
    private formBuilder: FormBuilder,
    private location: Location,
    private accountService: AccountsService,
    private transactionsService: TransactionsService,
    private adapterPurchase: MakePurchaseCommandAdapter,
    private adapterTransfer: TransferMoneyCommandAdapter
  ) { }

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
      // eslint-disable-next-line @typescript-eslint/no-unused-vars
      filter(([params, accounts]) => accounts.length > 0),
      tap(([params]) => {
        const fromAccount =
          params[accountIdParamName] ??
          this.accountService.findOneByIndex(0)?.id;
        const toAccount = this.accountService.findOneByIndex(1)?.id;
        this.formPurchase.patchValue({
          date: startOfToday(),
          account: fromAccount,
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

    this.store
      .select(CategoriesFeature.selectByIndex(0))
      .pipe(filterNullish())
      .subscribe(item => {
        this.formPurchase.patchValue({
          category: item.id,
        });
      });
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
      .subscribe(() => this.location.back());
  }

  onTransfer(): void {
    this.transactionsService
      .transfer(this.adapterTransfer.adapt(this.formTransfer.value))
      .subscribe(() => this.location.back());
  }

  goBack(): void {
    this.location.back();
  }
}
