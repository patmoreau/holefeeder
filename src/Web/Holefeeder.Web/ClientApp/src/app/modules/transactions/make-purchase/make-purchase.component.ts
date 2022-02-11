import {Component, OnInit} from '@angular/core';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {ActivatedRoute} from '@angular/router';
import {Location} from '@angular/common';
import {AccountsInfoService} from '@app/core/services/account-info.service';
import {startOfToday} from 'date-fns';
import {CategoriesService} from '@app/core/services/categories.service';
import {TransferMoneyCommandAdapter} from "@app/core/models/transfer-money-command.model";
import {combineLatest, filter, Observable, tap} from "rxjs";
import {TransactionsService} from "@app/core/services/transactions.service";
import {MakePurchaseCommandAdapter} from "@app/core/models/make-purchase-command.model";

const accountIdParamName = 'accountId';

@Component({
  selector: 'app-make-purchase',
  templateUrl: './make-purchase.component.html',
  styleUrls: ['./make-purchase.component.scss']
})
export class MakePurchaseComponent implements OnInit {

  formPurchase!: FormGroup;
  formTransfer!: FormGroup;

  values$!: Observable<[any ,any]>;

  constructor(
    private route: ActivatedRoute,
    private formBuilder: FormBuilder,
    private location: Location,
    private accountService: AccountsInfoService,
    private categoriesService: CategoriesService,
    private transactionsService: TransactionsService,
    private adapterPurchase: MakePurchaseCommandAdapter,
    private adapterTransfer: TransferMoneyCommandAdapter
  ) {
  }

  ngOnInit(): void {

    this.formPurchase = this.formBuilder.group({
      amount: ['', [Validators.required, Validators.min(0)]],
      date: [''],
      account: ['', [Validators.required]],
      category: ['', [Validators.required]],
      description: [''],
      tags: this.formBuilder.array([])
    });

    this.formTransfer = this.formBuilder.group({
      amount: ['', [Validators.required, Validators.min(0)]],
      date: [''],
      fromAccount: ['', [Validators.required]],
      toAccount: ['', [Validators.required]],
      description: ['']
    });

    this.values$ = combineLatest([
      this.route.params,
      this.accountService.accounts$
    ]).pipe(
      filter(([_, accounts]) => accounts.length > 0),
      tap(([params, accounts]) => {
        let fromAccount = params[accountIdParamName] ?? this.accountService.findOneByIndex(0)?.id;
        let toAccount = this.accountService.findOneByIndex(1)?.id;
        console.log(fromAccount);
        this.formPurchase.patchValue({
          date: startOfToday(),
          account: fromAccount,
          category: this.categoriesService.findOneByIndex(0)?.id
        });
        this.formTransfer.patchValue({
          date: startOfToday(),
          fromAccount: fromAccount,
          toAccount: toAccount
        });
      })
    )
  }

  onMakePurchase(): void {
    this.transactionsService.makePurchase(
      this.adapterPurchase.adapt(this.formPurchase.value))
      .subscribe(_ => this.location.back());
  }

  onTransfer(): void {
    this.transactionsService.transfer(
      this.adapterTransfer.adapt(this.formTransfer.value))
      .subscribe(_ => this.location.back());
  }

  goBack(): void {
    this.location.back();
  }
}
