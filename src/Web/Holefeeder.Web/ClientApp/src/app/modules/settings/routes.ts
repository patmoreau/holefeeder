import { Routes } from '@angular/router';
import { AutoLoginAllRoutesGuard } from 'angular-auth-oidc-client';

export const SETTINGS_ROUTES: Routes = [
  { path: '', redirectTo: '/settings/account', pathMatch: 'full' },
  {
    path: '',
    loadComponent: () =>
      import('./settings.component').then(m => m.SettingsComponent),
    canActivate: [AutoLoginAllRoutesGuard],
    children: [
      {
        path: 'account',
        loadComponent: () =>
          import('./account/account.component').then(m => m.AccountComponent),
        canActivateChild: [AutoLoginAllRoutesGuard],
      },
      {
        path: 'general',
        loadComponent: () =>
          import('./general/general.component').then(m => m.GeneralComponent),
        canActivateChild: [AutoLoginAllRoutesGuard],
      },
    ],
  },
];
