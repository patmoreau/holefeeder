import {CommonModule} from '@angular/common';
import {CUSTOM_ELEMENTS_SCHEMA, NgModule} from '@angular/core';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import {AccountsListComponent} from './accounts-list/accounts-list.component';
import {AccountDetailsComponent} from './account-details/account-details.component';
import {AccountsRoutingModule} from './accounts-routing.module';
import {AccountEditComponent} from './account-edit/account-edit.component';
import {SharedModule} from '@app/shared/shared.module';
import {AccountsComponent} from './accounts/accounts.component';
import {AccountUpcomingComponent} from './account-upcoming/account-upcoming.component';
import {AccountTransactionsComponent} from './account-transactions/account-transactions.component';
import {OpenAccountAdapter} from './models/open-account-command.model';
import {ModifyAccountAdapter} from './models/modify-account-command.model';
import {OpenAccountComponent} from './open-account/open-account.component';
import {ModifyAccountComponent} from './modify-account/modify-account.component';
import {AccountCommandsService} from "@app/modules/accounts/services/account-commands.service";

const COMPONENTS = [
  AccountsListComponent,
  AccountEditComponent,
  AccountDetailsComponent,
  AccountsComponent,
  AccountUpcomingComponent,
  AccountTransactionsComponent,
  OpenAccountComponent,
  ModifyAccountComponent
];

@NgModule({
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    SharedModule,
    AccountsRoutingModule
  ],
  declarations: [COMPONENTS],
  providers: [
    OpenAccountAdapter,
    ModifyAccountAdapter,
    AccountCommandsService
  ],
  schemas: [CUSTOM_ELEMENTS_SCHEMA]
})
export class AccountsModule {
}
