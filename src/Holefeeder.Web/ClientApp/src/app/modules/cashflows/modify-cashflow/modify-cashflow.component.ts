import { CommonModule, Location } from '@angular/common';
import {
  Component,
  ElementRef,
  inject,
  OnInit,
  ViewChild,
} from '@angular/core';
import {
  FormArray,
  FormBuilder,
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { ActivatedRoute, Params } from '@angular/router';
import { ModifyCashflowCommandAdapter } from '@app/core/adapters';
import {
  AccountsService,
  CashflowsService,
  ModalService,
} from '@app/core/services';
import {
  DatePickerComponent,
  DecimalInputComponent,
  LoaderComponent,
  TagsInputComponent,
} from '@app/shared/components';
import { filterNullish, filterTrue } from '@app/shared/helpers';
import {
  Account,
  CashflowDetail,
  Category,
  DateIntervalTypeNames,
} from '@app/shared/models';
import { NgbModalRef } from '@ng-bootstrap/ng-bootstrap';
import { Observable, of, switchMap, tap } from 'rxjs';
import { Store } from '@ngrx/store';
import { AppState, CategoriesFeature } from '@app/core/store';

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
    DatePickerComponent,
    LoaderComponent,
    DecimalInputComponent
  ]
})
export class ModifyCashflowComponent implements OnInit {
  form!: FormGroup;

  cashflowId!: string;

  values$: Observable<CashflowDetail> = of({} as CashflowDetail);

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

  private readonly store = inject(Store<AppState>);

  constructor(
    private route: ActivatedRoute,
    private formBuilder: FormBuilder,
    private location: Location,
    private accountsService: AccountsService,
    private cashflowsService: CashflowsService,
    private adapter: ModifyCashflowCommandAdapter,
    private modalService: ModalService
  ) { }

  get amount(): FormControl {
    return this.form.get('amount') as FormControl;
  }

  get tags(): FormArray {
    return this.form.get('tags') as FormArray;
  }

  async ngOnInit() {
    this.accounts$ = this.accountsService.activeAccounts$;
    this.categories$ = this.store.select(CategoriesFeature.selectAll);

    this.form = this.formBuilder.group({
      amount: ['', [Validators.required, Validators.min(0)]],
      intervalType: [{ value: '', disabled: true }],
      frequency: [{ value: '', disabled: true }],
      effectiveDate: [{ value: '' }, [Validators.required]],
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
      .subscribe(() => this.location.back());
  }

  onCancel() {
    this.modalService
      .delete('Are you sure you want to cancel this cashflow?')
      .pipe(
        filterTrue(),
        switchMap(() => this.cashflowsService.cancel(this.cashflowId))
      )
      .subscribe(() => this.location.back());
  }

  goBack(): void {
    this.location.back();
  }
}
