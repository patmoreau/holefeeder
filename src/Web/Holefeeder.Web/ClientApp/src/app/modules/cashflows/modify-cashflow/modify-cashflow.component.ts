import { CommonModule, Location } from '@angular/common';
import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import {
  FormArray,
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { ActivatedRoute, Params } from '@angular/router';
import { ModifyCashflowCommandAdapter } from '@app/core/adapters';
import {
  AccountsService,
  CashflowsService,
  CategoriesService,
  ModalService,
} from '@app/core/services';
import {
  DatePickerComponent,
  LoaderComponent,
  TagsInputComponent,
} from '@app/shared/components';
import { AutofocusDirective } from '@app/shared/directives';
import { filterNullish, filterTrue } from '@app/shared/helpers';
import {
  Account,
  CashflowDetail,
  Category,
  DateIntervalTypeNames,
} from '@app/shared/models';
import { NgbModalRef } from '@ng-bootstrap/ng-bootstrap';
import { Observable, switchMap, tap } from 'rxjs';

const cashflowIdParamName = 'cashflowId';

@Component({
  selector: 'app-cashflow-edit',
  templateUrl: './modify-cashflow.component.html',
  styleUrls: ['./modify-cashflow.component.scss'],
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    TagsInputComponent,
    AutofocusDirective,
    DatePickerComponent,
    LoaderComponent,
  ],
})
export class ModifyCashflowComponent implements OnInit {
  form!: FormGroup;

  cashflowId!: string;

  values$!: Observable<any>;

  @ViewChild('confirm', { static: true })
  confirmModalElement!: ElementRef;
  confirmModal!: NgbModalRef;
  confirmMessages!: string;

  account!: string;
  cashflow!: CashflowDetail;

  accounts$!: Observable<Account[]>;
  categories$!: Observable<Category[]>;

  isLoaded = false;

  intervalTypesNames = DateIntervalTypeNames;

  constructor(
    private route: ActivatedRoute,
    private formBuilder: FormBuilder,
    private location: Location,
    private accountsService: AccountsService,
    private categoriesService: CategoriesService,
    private cashflowsService: CashflowsService,
    private adapter: ModifyCashflowCommandAdapter,
    private modalService: ModalService
  ) {}

  get tags(): FormArray {
    return this.form.get('tags') as FormArray;
  }

  async ngOnInit() {
    this.accounts$ = this.accountsService.activeAccounts$;
    this.categories$ = this.categoriesService.categories$;

    this.form = this.formBuilder.group({
      amount: ['', [Validators.required, Validators.min(0)]],
      intervalType: [{ value: '', disabled: true }],
      frequency: [{ value: '', disabled: true }],
      effectiveDate: [{ value: '', disabled: true }, [Validators.required]],
      account: [{ value: '', disabled: true }, [Validators.required]],
      category: [{ value: '', disabled: true }, [Validators.required]],
      description: ['', [Validators.required]],
      tags: this.formBuilder.array([]),
    });

    this.values$ = this.route.params.pipe(
      switchMap((params: Params) =>
        this.cashflowsService.findById(params[cashflowIdParamName])
      ),
      filterNullish(),
      tap((cashflow: CashflowDetail) => {
        this.cashflowId = cashflow.id;
        this.form.patchValue({
          amount: cashflow.amount,
          intervalType: cashflow.intervalType,
          frequency: cashflow.frequency,
          effectiveDate: cashflow.effectiveDate,
          account: cashflow.account.id,
          category: cashflow.category.id,
          description: cashflow.description,
        });
        if (cashflow.tags) {
          const tags = this.form.get('tags') as FormArray;
          cashflow.tags.forEach(t => tags.push(this.formBuilder.control(t)));
        }
      })
    );
  }

  onSubmit() {
    this.cashflowsService
      .modify(
        this.adapter.adapt(
          Object.assign({}, this.form.value, {
            id: this.cashflowId,
          })
        )
      )
      .subscribe(_ => this.location.back());
  }

  onCancel() {
    this.modalService
      .delete('Are you sure you want to cancel this cashflow?')
      .pipe(
        filterTrue(),
        switchMap(_ => this.cashflowsService.cancel(this.cashflowId))
      )
      .subscribe(_ => this.location.back());
  }

  goBack(): void {
    this.location.back();
  }
}