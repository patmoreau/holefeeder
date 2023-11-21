import { Routes } from '@angular/router';
import { authenticationGuard } from "@app/core/auth/authorization.guard";

export const SETTINGS_ROUTES: Routes = [
  { path: '', redirectTo: '/settings/account', pathMatch: 'full' },
  {
    path: '',
    loadComponent: () =>
      import('./settings.component').then(m => m.SettingsComponent),
    canActivate: [authenticationGuard],
    children: [
      {
        path: 'account',
        loadComponent: () =>
          import('./account/account.component').then(m => m.AccountComponent),
        canActivateChild: [authenticationGuard],
      },
      {
        path: 'general',
        loadComponent: () =>
          import('./general/general.component').then(m => m.GeneralComponent),
        canActivateChild: [authenticationGuard],
      },
    ],
  },
];
