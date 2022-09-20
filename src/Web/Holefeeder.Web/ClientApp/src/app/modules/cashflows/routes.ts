import { Routes } from '@angular/router';
import { AutoLoginAllRoutesGuard } from 'angular-auth-oidc-client';

export const CASHFLOWS_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./cashflows/cashflows.component').then(m => m.CashflowsComponent),
    children: [
      {
        path: '',
        loadComponent: () =>
          import('./cashflows-list/cashflows-list.component').then(
            m => m.CashflowsListComponent
          ),
        canActivate: [AutoLoginAllRoutesGuard],
        runGuardsAndResolvers: 'always',
        pathMatch: 'full',
      },
      {
        path: ':cashflowId',
        loadComponent: () =>
          import('./modify-cashflow/modify-cashflow.component').then(
            m => m.ModifyCashflowComponent
          ),
        canActivate: [AutoLoginAllRoutesGuard],
      },
    ],
  },
];
