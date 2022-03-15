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
import {ModalService} from './services/modal.service';
import {ToastViewComponent} from "@app/shared/components/toast-view/toast-view.component";

const COMPONENTS = [
  DateViewComponent,
  ToastViewComponent,
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
    NgbModule
  ],
  exports: [COMPONENTS],
  declarations: [COMPONENTS],
  providers: [
    SubscriberService,
    ModalService
  ]
})
export class SharedModule {
}
