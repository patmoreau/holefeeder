import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AuthGuardService } from '@app/auth/services/auth-guard.service';
import { AccountsListComponent } from './accounts-list/accounts-list.component';
import { AccountEditComponent } from './account-edit/account-edit.component';
import { AccountDetailsComponent } from './account-details/account-details.component';
import { AccountsComponent } from './accounts/accounts.component';
import { AccountUpcomingComponent } from './account-upcoming/account-upcoming.component';
import { AccountTransactionsComponent } from './account-transactions/account-transactions.component';

const routes: Routes = [
  {
    path: '',
    component: AccountsComponent,
    children: [
      {
        path: '',
        component: AccountsListComponent,
        canActivate: [AuthGuardService],
        runGuardsAndResolvers: 'always',
        pathMatch: 'full'
      },
      {
        path: 'create',
        component: AccountEditComponent,
        canActivate: [AuthGuardService],
      },
      {
        path: ':accountId',
        component: AccountDetailsComponent,
        canActivate: [AuthGuardService],
        children: [
          { path: '', redirectTo: 'upcoming', pathMatch: 'full' },
          {
            path: 'edit',
            component: AccountEditComponent,
            canActivate: [AuthGuardService],
          },
          {
            path: 'upcoming',
            component: AccountUpcomingComponent,
            canActivate: [AuthGuardService],
          },
          {
            path: 'transactions',
            component: AccountTransactionsComponent,
            canActivate: [AuthGuardService],
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
