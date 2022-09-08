import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { DatePickerComponent } from '@app/shared/components/date-picker/date-picker.component';
import { DateViewComponent } from '@app/shared/components/date-view/date-view.component';
import { LoaderComponent } from '@app/shared/components/loader/loader.component';
import { ConfirmDialogComponent } from '@app/shared/components/modals/confirm-dialog/confirm-dialog.component';
import { DeleteDialogComponent } from '@app/shared/components/modals/delete-dialog/delete-dialog.component';
import { InputDialogComponent } from '@app/shared/components/modals/input-dialog/input-dialog.component';
import { MessageDialogComponent } from '@app/shared/components/modals/message-dialog/message-dialog.component';
import { RecurringCashflowComponent } from '@app/shared/components/recurring-cashflow/recurring-cashflow.component';
import { TagsInputComponent } from '@app/shared/components/tags-input/tags-input.component';
import { ToastViewComponent } from '@app/shared/components/toast-view/toast-view.component';
import { TransactionListItemComponent } from '@app/shared/components/transaction-list-item/transaction-list-item.component';
import { TransactionsListComponent } from '@app/shared/components/transactions-list/transactions-list.component';
import { UpcomingListComponent } from '@app/shared/components/upcoming-list/upcoming-list.component';
import { AutofocusDirective } from '@app/shared/directives/autofocus.directive';
import {
  DateValidator,
  DateValidatorDirective,
} from '@app/shared/directives/date-validator.directive';
import { SubscriberService } from '@app/shared/services/subscriber.service';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';

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
  DatePickerComponent,
  DateValidatorDirective,
];

@NgModule({
  imports: [CommonModule, FormsModule, ReactiveFormsModule, NgbModule],
  exports: [COMPONENTS],
  declarations: [COMPONENTS],
  providers: [SubscriberService, DateValidator],
})
export class SharedModule {}
