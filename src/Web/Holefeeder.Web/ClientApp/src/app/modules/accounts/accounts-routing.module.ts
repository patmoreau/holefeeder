import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { MsalGuard } from '@azure/msal-angular';
import { AccountDetailsComponent } from './account-details/account-details.component';
import { AccountTransactionsComponent } from './account-transactions/account-transactions.component';
import { AccountUpcomingComponent } from './account-upcoming/account-upcoming.component';
import { AccountsListComponent } from './accounts-list/accounts-list.component';
import { AccountsComponent } from './accounts/accounts.component';
import { ModifyAccountComponent } from './modify-account/modify-account.component';
import { OpenAccountComponent } from './open-account/open-account.component';

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
        pathMatch: 'full',
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
        children: [
          { path: '', redirectTo: 'upcoming', pathMatch: 'full' },
          {
            path: 'upcoming',
            component: AccountUpcomingComponent,
            canActivate: [MsalGuard],
          },
          {
            path: 'transactions',
            component: AccountTransactionsComponent,
            canActivate: [MsalGuard],
          },
        ],
      },
    ],
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class AccountsRoutingModule {}
