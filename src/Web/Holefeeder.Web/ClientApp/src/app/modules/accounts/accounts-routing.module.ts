import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AutoLoginAllRoutesGuard } from 'angular-auth-oidc-client';
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
        canActivate: [AutoLoginAllRoutesGuard],
        runGuardsAndResolvers: 'always',
        pathMatch: 'full',
      },
      {
        path: 'create',
        component: OpenAccountComponent,
        canActivate: [AutoLoginAllRoutesGuard],
      },
      {
        path: ':accountId/edit',
        component: ModifyAccountComponent,
        canActivate: [AutoLoginAllRoutesGuard],
      },
      {
        path: ':accountId',
        component: AccountDetailsComponent,
        canActivate: [AutoLoginAllRoutesGuard],
        children: [
          { path: '', redirectTo: 'upcoming', pathMatch: 'full' },
          {
            path: 'upcoming',
            component: AccountUpcomingComponent,
            canActivate: [AutoLoginAllRoutesGuard],
          },
          {
            path: 'transactions',
            component: AccountTransactionsComponent,
            canActivate: [AutoLoginAllRoutesGuard],
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
