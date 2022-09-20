import { CommonModule, Location } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import {
  FormArray,
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { ActivatedRoute, Data } from '@angular/router';
import { PayCashflowCommandAdapter, Upcoming } from '@app/core/models';
import { TransactionsService, UpcomingService } from '@app/core/services';
import { map, Observable, tap } from 'rxjs';
import { TransactionEditComponent } from '@app/modules/transactions/transaction-edit/transaction-edit.component';
import { LoaderComponent } from '@app/shared';

const cashflowIdParamName = 'cashflowId';

@Component({
  selector: 'app-pay-cashflow',
  templateUrl: './pay-cashflow.component.html',
  styleUrls: ['./pay-cashflow.component.scss'],
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    TransactionEditComponent,
    LoaderComponent,
  ],
})
export class PayCashflowComponent implements OnInit {
  form!: FormGroup;

  cashflow!: Upcoming;

  values$!: Observable<any>;

  constructor(
    private route: ActivatedRoute,
    private formBuilder: FormBuilder,
    private location: Location,
    private cashflowsService: UpcomingService,
    private transactionsService: TransactionsService,
    private adapter: PayCashflowCommandAdapter
  ) {}

  ngOnInit(): void {
    this.form = this.formBuilder.group({
      amount: ['', [Validators.required, Validators.min(0)]],
      date: ['', [Validators.required]],
      account: [{ value: '', disabled: true }, [Validators.required]],
      category: [{ value: '', disabled: true }, [Validators.required]],
      description: [{ value: '', disabled: true }],
      tags: this.formBuilder.array([]),
    });

    this.values$ = this.route.data.pipe(
      map((data: Data) => data['cashflow']),
      tap(cashflow => {
        this.cashflow = cashflow;
        this.form.patchValue({
          amount: cashflow.amount,
          date: cashflow.date,
          account: cashflow.account.id,
          category: cashflow.category.id,
          description: cashflow.description,
        });
        if (cashflow.tags) {
          const tags = this.form.get('tags') as FormArray;
          cashflow.tags.forEach((tag: string) =>
            tags.push(this.formBuilder.control(tag))
          );
        }
      })
    );
  }

  onSubmit(): void {
    this.transactionsService
      .payCashflow(
        this.adapter.adapt(
          Object.assign({}, this.form.value, {
            cashflow: this.cashflow.id,
            cashflowDate: this.cashflow.date,
          })
        )
      )
      .subscribe(_ => this.location.back());
  }

  goBack(): void {
    this.location.back();
  }
}
