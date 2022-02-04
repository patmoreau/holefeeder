import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { AccountsListComponent } from './accounts-list/accounts-list.component';
import { AccountDetailsComponent } from './account-details/account-details.component';
import { AccountsRoutingModule } from './accounts-routing.module';
import { AccountEditComponent } from './account-edit/account-edit.component';
import { SharedModule } from '@app/shared/shared.module';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { AccountsComponent } from './accounts/accounts.component';
import { AccountUpcomingComponent } from './account-upcoming/account-upcoming.component';
import { AccountTransactionsComponent } from './account-transactions/account-transactions.component';
import { AccountsApiService } from '@app/modules/accounts/services/api/accounts-api.service';
import { AccountsService } from './services/accounts.service';
import { OpenAccountAdapter } from './models/open-account-command.model';
import { ModifyAccountAdapter } from './models/modify-account-command.model';
import { AccountAdapter } from './models/account.model';

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
    AccountsRoutingModule
  ],
  declarations: [COMPONENTS],
  providers: [AccountsService, AccountsApiService, AccountAdapter, OpenAccountAdapter, ModifyAccountAdapter],
  schemas: [CUSTOM_ELEMENTS_SCHEMA]
})
export class AccountsModule { }
