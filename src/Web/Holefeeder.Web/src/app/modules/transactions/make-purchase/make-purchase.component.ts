import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Params } from '@angular/router';
import { Observable, tap } from 'rxjs';
import { Location } from '@angular/common';
import { AccountsInfoService } from '@app/core/services/account-info.service';
import { startOfToday } from 'date-fns';
import { CategoriesService } from '@app/core/services/categories.service';
import { TransactionsService } from '../../../core/services/transactions.service';
import { MakePurchaseCommandAdapter } from '../../../core/models/make-purchase-command.model';

const accountIdParamName = 'accountId';

@Component({
  selector: 'app-make-purchase',
  templateUrl: './make-purchase.component.html',
  styleUrls: ['./make-purchase.component.scss']
})
export class MakePurchaseComponent implements OnInit {

  form!: FormGroup;

  values$!: Observable<any>;

  constructor(
    private route: ActivatedRoute,
    private formBuilder: FormBuilder,
    private location: Location,
    private accountService: AccountsInfoService,
    private categoriesService: CategoriesService,
    private transactionsService: TransactionsService,
    private adapter: MakePurchaseCommandAdapter
  ) { }

  ngOnInit(): void {

    this.form = this.formBuilder.group({
      amount: ['', [Validators.required, Validators.min(0)]],
      date: [''],
      account: ['', [Validators.required]],
      category: ['', [Validators.required]],
      description: [''],
      tags: this.formBuilder.array([])
    });

    this.values$ = this.route.params.pipe(
      tap((params: Params) => {
        let accountId: string = params[accountIdParamName] ?? this.accountService.findOneByIndex(0)?.id;
        this.form.patchValue({
          date: startOfToday(),
          account: accountId,
          category: this.categoriesService.findOneByIndex(0)?.id
        });
      }));
  }

  onSubmit(): void {
    this.transactionsService.makePurchase(
      this.adapter.adapt(this.form.value))
      .subscribe(_ => this.location.back());
  }

  goBack(): void {
    this.location.back();
  }
}
