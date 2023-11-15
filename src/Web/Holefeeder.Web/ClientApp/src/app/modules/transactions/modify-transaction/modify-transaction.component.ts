import { CommonModule, Location } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import {
  FormArray,
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { ActivatedRoute, Params } from '@angular/router';
import { ModifyTransactionCommandAdapter } from '@app/core/adapters';
import { ModalService, TransactionsService } from '@app/core/services';
import { TransactionEditComponent } from '@app/modules/transactions/transaction-edit/transaction-edit.component';
import { LoaderComponent } from '@app/shared/components';
import { filterTrue } from '@app/shared/helpers';
import { TransactionDetail } from '@app/shared/models';
import { filter, Observable, switchMap, tap } from 'rxjs';

const transactionIdParamName = 'transactionId';

@Component({
  selector: 'app-modify-transaction',
  templateUrl: './modify-transaction.component.html',
  styleUrls: ['./modify-transaction.component.scss'],
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    TransactionEditComponent,
    LoaderComponent,
  ],
})
export class ModifyTransactionComponent implements OnInit {
  form!: FormGroup;

  transactionId!: string;

  values$!: Observable<TransactionDetail>;

  constructor(
    private route: ActivatedRoute,
    private formBuilder: FormBuilder,
    private location: Location,
    private transactionsService: TransactionsService,
    private adapter: ModifyTransactionCommandAdapter,
    private modalService: ModalService
  ) {}

  ngOnInit(): void {
    this.form = this.formBuilder.group({
      amount: ['', [Validators.required, Validators.min(0)]],
      date: ['', [Validators.required]],
      account: ['', [Validators.required]],
      category: ['', [Validators.required]],
      description: [''],
      tags: this.formBuilder.array([]),
    });

    this.values$ = this.route.params.pipe(
      switchMap((params: Params) =>
        this.transactionsService.findById(params[transactionIdParamName])
      ),
      filter((transaction: TransactionDetail) => transaction !== undefined),
      tap((transaction: TransactionDetail) => {
        this.transactionId = transaction.id;
        this.form.patchValue({
          amount: transaction.amount,
          date: transaction.date,
          account: transaction.account.id,
          category: transaction.category.id,
          description: transaction.description,
        });
        if (transaction.tags) {
          const tags = this.form.get('tags') as FormArray;
          transaction.tags.forEach(t => tags.push(this.formBuilder.control(t)));
        }
      })
    );
  }

  onSubmit() {
    this.transactionsService
      .modify(
        this.adapter.adapt(
          Object.assign({}, this.form.value, {
            id: this.transactionId,
          })
        )
      )
      .subscribe(() => this.location.back());
  }

  onDelete() {
    this.modalService
      .delete('Are you sure you want to delete this transaction?')
      .pipe(
        filterTrue(),
        switchMap(() => this.transactionsService.delete(this.transactionId))
      )
      .subscribe(() => this.location.back());
  }

  goBack(): void {
    this.location.back();
  }
}
