import { Routes } from '@angular/router';

export const HOME_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () => import('./home.component').then(m => m.HomeComponent),
    children: [
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
      {
        path: 'dashboard',
        loadChildren: () =>
          import('../dashboard/routes').then(m => m.DASHBOARD_ROUTES),
      },
      {
        path: 'accounts',
        loadChildren: () =>
          import('../accounts/routes').then(m => m.ACCOUNTS_ROUTES),
      },
      {
        path: 'settings',
        loadChildren: () =>
          import('../settings/routes').then(m => m.SETTINGS_ROUTES),
      },
      {
        path: 'cashflows',
        loadChildren: () =>
          import('../cashflows/routes').then(m => m.CASHFLOWS_ROUTES),
      },
      {
        path: 'statistics',
        loadChildren: () =>
          import('../statistics/routes').then(m => m.STATISTICS_ROUTES),
      },
      {
        path: 'transactions',
        loadChildren: () =>
          import('../transactions/routes').then(m => m.TRANSACTIONS_ROUTES),
      },
    ],
  },
  {
    path: '**',
    loadComponent: () =>
      import('@app/shared/components').then(m => m.ErrorNotfoundComponent),
  },
];
