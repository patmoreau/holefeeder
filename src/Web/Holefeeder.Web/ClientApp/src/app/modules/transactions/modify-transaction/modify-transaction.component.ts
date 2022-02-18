import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators, FormArray } from '@angular/forms';
import { ActivatedRoute, Params } from '@angular/router';
import { Location } from '@angular/common';
import { Observable, switchMap, filter, tap } from 'rxjs';
import { ModalService } from '@app/shared/services/modal.service';
import { TransactionsService } from '@app/core/services/transactions.service';
import { ModifyTransactionCommandAdapter } from '@app/core/models/modify-transaction-command.model';
import { TransactionDetail } from '@app/core/models/transaction-detail.model';

const transactionIdParamName = 'transactionId';

@Component({
  selector: 'app-modify-transaction',
  templateUrl: './modify-transaction.component.html',
  styleUrls: ['./modify-transaction.component.scss']
})
export class ModifyTransactionComponent implements OnInit {

  form!: FormGroup;

  transactionId!: string;

  values$!: Observable<any>;

  constructor(
    private route: ActivatedRoute,
    private formBuilder: FormBuilder,
    private location: Location,
    private transactionsService: TransactionsService,
    private adapter: ModifyTransactionCommandAdapter,
    private modalService: ModalService,
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
      switchMap((params: Params) => this.transactionsService.findById(params[transactionIdParamName])),
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
      }));
  }

  onSubmit() {
    this.transactionsService.modify(
      this.adapter.adapt(Object.assign({}, this.form.value, {
        id: this.transactionId
      }))).subscribe(_ => this.location.back());
  }

  onDelete(content: any) {
    this.modalService.open(content, { ariaLabelledBy: 'modal-delete-title' })
      .pipe(
        switchMap(_ => this.transactionsService.delete(this.transactionId))
      )
      .subscribe(_ => this.location.back());
  }

  goBack(): void {
    this.location.back();
  }
}

