import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { library } from '@fortawesome/fontawesome-svg-core';
import { fas } from '@fortawesome/free-solid-svg-icons';
import { far } from '@fortawesome/free-regular-svg-icons';
import { DateViewComponent } from './components/date-view/date-view.component';
import { AccountsService } from './services/accounts.service';
import { TagsInputComponent } from './tags-input/tags-input.component';
import { LoaderComponent } from './components/loader/loader.component';
import { TransactionsService } from './services/transactions.service';
import { CategoriesService } from './services/categories.service';
import { TransactionsListComponent } from './components/transactions-list/transactions-list.component';
import { CurrencyComponent } from './components/currency/currency.component';
import { CurrencyDirective } from './components/currency/currency.directive';
import { ApiService } from './services/api.service';
import { SubscriberService } from './services/subscriber.service';
import { TransactionListItemComponent } from './components/transaction-list-item/transaction-list-item.component';
import { UpcomingListComponent } from './components/upcoming-list/upcoming-list.component';
import { CashflowsService } from './services/cashflows.service';
import { AutofocusDirective } from './directives/autofocus.directive';

library.add(fas, far);

const COMPONENTS = [
  CurrencyComponent,
  DateViewComponent,
  CurrencyDirective,
  TagsInputComponent,
  LoaderComponent,
  TransactionListItemComponent,
  TransactionsListComponent,
  UpcomingListComponent,
  AutofocusDirective
];

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    FontAwesomeModule,
    NgbModule
  ],
  exports: [COMPONENTS],
  declarations: [COMPONENTS],
  providers: [
    ApiService,
    SubscriberService,
    CashflowsService,
    AccountsService,
    CategoriesService,
    TransactionsService
  ]
})
export class SharedModule { }
