import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { AccountsListComponent } from './accounts-list/accounts-list.component';
import { AccountDetailsComponent } from './account-details/account-details.component';
import { AccountsService } from '../shared/services/accounts.service';
import { AccountsRoutingModule } from './accounts-routing.module';
import { AccountEditComponent } from './account-edit/account-edit.component';
import { SharedModule } from '@app/shared/shared.module';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { AccountsComponent } from './accounts/accounts.component';
import { AccountUpcomingComponent } from './account-upcoming/account-upcoming.component';
import { AccountTransactionsComponent } from './account-transactions/account-transactions.component';

const COMPONENTS = [
  AccountsListComponent,
  AccountEditComponent,
  AccountDetailsComponent,
  AccountsComponent,
  AccountUpcomingComponent,
  AccountTransactionsComponent
];

@NgModule({
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    NgbModule,
    SharedModule,
    FontAwesomeModule,
    AccountsRoutingModule
  ],
  declarations: [COMPONENTS],
  providers: [AccountsService],
  schemas: [CUSTOM_ELEMENTS_SCHEMA]
})
export class AccountsModule { }
