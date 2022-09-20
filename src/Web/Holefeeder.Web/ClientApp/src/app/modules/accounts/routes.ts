import { Routes } from '@angular/router';
import { AutoLoginAllRoutesGuard } from 'angular-auth-oidc-client';

export const ACCOUNTS_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./accounts/accounts.component').then(m => m.AccountsComponent),
    children: [
      {
        path: '',
        loadComponent: () =>
          import('./accounts-list/accounts-list.component').then(
            m => m.AccountsListComponent
          ),
        canActivate: [AutoLoginAllRoutesGuard],
        runGuardsAndResolvers: 'always',
        pathMatch: 'full',
      },
      {
        path: 'create',
        loadComponent: () =>
          import('./open-account/open-account.component').then(
            m => m.OpenAccountComponent
          ),
        canActivate: [AutoLoginAllRoutesGuard],
      },
      {
        path: ':accountId/edit',
        loadComponent: () =>
          import('./modify-account/modify-account.component').then(
            m => m.ModifyAccountComponent
          ),
        canActivate: [AutoLoginAllRoutesGuard],
      },
      {
        path: ':accountId',
        loadComponent: () =>
          import('./account-details/account-details.component').then(
            m => m.AccountDetailsComponent
          ),
        canActivate: [AutoLoginAllRoutesGuard],
        children: [
          { path: '', redirectTo: 'upcoming', pathMatch: 'full' },
          {
            path: 'upcoming',
            loadComponent: () =>
              import('./account-upcoming/account-upcoming.component').then(
                m => m.AccountUpcomingComponent
              ),
            canActivate: [AutoLoginAllRoutesGuard],
          },
          {
            path: 'transactions',
            loadComponent: () =>
              import(
                './account-transactions/account-transactions.component'
              ).then(m => m.AccountTransactionsComponent),
            canActivate: [AutoLoginAllRoutesGuard],
          },
        ],
      },
    ],
  },
];
