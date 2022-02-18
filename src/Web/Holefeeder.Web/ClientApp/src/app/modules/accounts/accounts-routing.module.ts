import {NgModule} from '@angular/core';
import {Routes, RouterModule} from '@angular/router';
import {AccountsListComponent} from './accounts-list/accounts-list.component';
import {AccountEditComponent} from './account-edit/account-edit.component';
import {AccountDetailsComponent} from './account-details/account-details.component';
import {AccountsComponent} from './accounts/accounts.component';
import {AccountUpcomingComponent} from './account-upcoming/account-upcoming.component';
import {AccountTransactionsComponent} from './account-transactions/account-transactions.component';
import {MsalGuard} from '@azure/msal-angular';
import {OpenAccountComponent} from './open-account/open-account.component';
import {ModifyAccountComponent} from './modify-account/modify-account.component';
import {AccountResolver} from "@app/modules/accounts/account.resolver";

const routes: Routes = [
  {
    path: '',
    component: AccountsComponent,
    children: [
      {
        path: '',
        component: AccountsListComponent,
        canActivate: [MsalGuard],
        runGuardsAndResolvers: 'always',
        pathMatch: 'full'
      },
      {
        path: 'create',
        component: OpenAccountComponent,
        canActivate: [MsalGuard],
      },
      {
        path: ':accountId/edit',
        component: ModifyAccountComponent,
        canActivate: [MsalGuard],
      },
      {
        path: ':accountId',
        component: AccountDetailsComponent,
        canActivate: [MsalGuard],
        resolve: {
          account: AccountResolver
        },
        children: [
          {path: '', redirectTo: 'upcoming'},
          {
            path: 'upcoming',
            component: AccountUpcomingComponent,
            canActivate: [MsalGuard],
          },
          {
            path: 'transactions',
            component: AccountTransactionsComponent,
            canActivate: [MsalGuard]
          }
        ]
      },
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AccountsRoutingModule {
}
