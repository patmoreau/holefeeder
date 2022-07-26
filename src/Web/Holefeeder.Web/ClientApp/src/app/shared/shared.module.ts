import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import {NgbModule} from '@ng-bootstrap/ng-bootstrap';
import {DateViewComponent} from './components/date-view/date-view.component';
import {TagsInputComponent} from './tags-input/tags-input.component';
import {LoaderComponent} from './components/loader/loader.component';
import {TransactionsListComponent} from './components/transactions-list/transactions-list.component';
import {SubscriberService} from './services/subscriber.service';
import {TransactionListItemComponent} from './components/transaction-list-item/transaction-list-item.component';
import {UpcomingListComponent} from './components/upcoming-list/upcoming-list.component';
import {AutofocusDirective} from './directives/autofocus.directive';
import {ToastViewComponent} from "@app/shared/components/toast-view/toast-view.component";
import {ConfirmDialogComponent} from "@app/shared/components/modals/confirm-dialog/confirm-dialog.component";
import {DeleteDialogComponent} from "@app/shared/components/modals/delete-dialog/delete-dialog.component";
import {MessageDialogComponent} from "@app/shared/components/modals/message-dialog/message-dialog.component";
import {InputDialogComponent} from "@app/shared/components/modals/input-dialog/input-dialog.component";
import {RecurringCashflowComponent} from './components/recurring-cashflow/recurring-cashflow.component';
import {DateSelectComponent} from './components/date-select/date-select.component';

const COMPONENTS = [
  DateViewComponent,
  ToastViewComponent,
  TagsInputComponent,
  LoaderComponent,
  TransactionListItemComponent,
  TransactionsListComponent,
  UpcomingListComponent,
  AutofocusDirective,
  ConfirmDialogComponent,
  InputDialogComponent,
  MessageDialogComponent,
  DeleteDialogComponent,
  RecurringCashflowComponent,
  DateSelectComponent
];

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    NgbModule
  ],
  exports: [COMPONENTS],
  declarations: [COMPONENTS],
  providers: [
    SubscriberService
  ]
})
export class SharedModule {
}
