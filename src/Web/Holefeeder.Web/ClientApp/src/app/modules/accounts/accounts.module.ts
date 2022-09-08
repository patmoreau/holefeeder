import { CommonModule } from '@angular/common';
import { CUSTOM_ELEMENTS_SCHEMA, NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { AccountCommandsService } from '@app/modules/accounts/services/account-commands.service';
import { SharedModule } from '@app/shared/shared.module';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { AccountDetailsComponent } from './account-details/account-details.component';
import { AccountEditComponent } from './account-edit/account-edit.component';
import { AccountTransactionsComponent } from './account-transactions/account-transactions.component';
import { AccountUpcomingComponent } from './account-upcoming/account-upcoming.component';
import { AccountsListComponent } from './accounts-list/accounts-list.component';
import { AccountsRoutingModule } from './accounts-routing.module';
import { AccountsComponent } from './accounts/accounts.component';
import { ModifyAccountAdapter } from './models/modify-account-command.model';
import { OpenAccountAdapter } from './models/open-account-command.model';
import { ModifyAccountComponent } from './modify-account/modify-account.component';
import { OpenAccountComponent } from './open-account/open-account.component';

const COMPONENTS = [
  AccountsListComponent,
  AccountEditComponent,
  AccountDetailsComponent,
  AccountsComponent,
  AccountUpcomingComponent,
  AccountTransactionsComponent,
  OpenAccountComponent,
  ModifyAccountComponent,
];

@NgModule({
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    SharedModule,
    NgbModule,
    AccountsRoutingModule,
  ],
  declarations: [COMPONENTS],
  providers: [OpenAccountAdapter, ModifyAccountAdapter, AccountCommandsService],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
})
export class AccountsModule {}
