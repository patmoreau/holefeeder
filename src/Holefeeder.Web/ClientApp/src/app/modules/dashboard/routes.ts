import { Routes } from '@angular/router';
import { DashboardHomeComponent } from './dashboard-home/dashboard-home.component';
import { authenticationGuard } from '@app/core/auth/authorization.guard';

export const DASHBOARD_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./dashboard/dashboard.component').then(m => m.DashboardComponent),
    children: [
      {
        path: '',
        component: DashboardHomeComponent,
        canActivate: [authenticationGuard],
      },
    ],
  },
];
