import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AccountsListComponent } from './accounts-list/accounts-list.component';
import { AccountEditComponent } from './account-edit/account-edit.component';
import { AccountDetailsComponent } from './account-details/account-details.component';
import { AccountsComponent } from './accounts/accounts.component';
import { AccountUpcomingComponent } from './account-upcoming/account-upcoming.component';
import { AccountTransactionsComponent } from './account-transactions/account-transactions.component';
import { MsalGuard } from '@azure/msal-angular';

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
        component: AccountEditComponent,
        canActivate: [MsalGuard],
      },
      {
        path: ':accountId',
        component: AccountDetailsComponent,
        canActivate: [MsalGuard],
        children: [
          { path: '', redirectTo: 'upcoming', pathMatch: 'full' },
          {
            path: 'edit',
            component: AccountEditComponent,
            canActivate: [MsalGuard],
          },
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
          // {
          //   path: 'transactions',
          //   loadChildren: './transactions/transactions.module#TransactionsModule', canActivate: [AuthGuardService]
          // },
        ]
      },
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AccountsRoutingModule { }
