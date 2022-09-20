import { Routes } from '@angular/router';
import { AutoLoginAllRoutesGuard } from 'angular-auth-oidc-client';
import { DashboardHomeComponent } from './dashboard-home/dashboard-home.component';

export const DASHBOARD_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./dashboard/dashboard.component').then(m => m.DashboardComponent),
    canActivate: [AutoLoginAllRoutesGuard],
    children: [
      {
        path: '',
        component: DashboardHomeComponent,
        canActivate: [AutoLoginAllRoutesGuard],
      },
    ],
  },
];
