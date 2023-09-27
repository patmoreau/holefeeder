import { CommonModule } from '@angular/common';
import { Component, inject, OnInit } from '@angular/core';
import {
  FormArray,
  FormGroup,
  FormGroupDirective,
  ReactiveFormsModule,
} from '@angular/forms';
import { AccountsService } from '@app/core/services';
import {
  DatePickerComponent,
  TagsInputComponent,
} from '@app/shared/components';
import { AutofocusDirective } from '@app/shared/directives';
import { AccountInfo, Category } from '@app/shared/models';
import { combineLatest, Observable } from 'rxjs';
import { Store } from '@ngrx/store';
import { AppState, CategoriesFeature } from '@app/core/store';

@Component({
  selector: 'app-transaction-edit',
  templateUrl: './transaction-edit.component.html',
  styleUrls: ['./transaction-edit.component.scss'],
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    DatePickerComponent,
    TagsInputComponent,
    AutofocusDirective,
  ],
})
export class TransactionEditComponent implements OnInit {
  form!: FormGroup;

  values$!: Observable<{
    accounts: AccountInfo[];
    categories: Category[];
  }>;

  private readonly store = inject(Store<AppState>);
  constructor(
    private rootFormGroup: FormGroupDirective,
    private accountsService: AccountsService
  ) {}

  get tags(): FormArray {
    return this.form.get('tags') as FormArray;
  }

  ngOnInit() {
    this.form = this.rootFormGroup.control;

    this.values$ = combineLatest({
      accounts: this.accountsService.activeAccounts$,
      categories: this.store.select(CategoriesFeature.selectAll),
    });
  }
}
