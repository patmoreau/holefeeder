import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AutoLoginAllRoutesGuard } from 'angular-auth-oidc-client';
import { AccountComponent } from './account/account.component';
import { GeneralComponent } from './general/general.component';
import { SettingsComponent } from './settings.component';

const settingsRoutes: Routes = [
  { path: '', redirectTo: '/settings/account', pathMatch: 'full' },
  {
    path: '',
    component: SettingsComponent,
    canActivate: [AutoLoginAllRoutesGuard],
    children: [
      {
        path: 'account',
        component: AccountComponent,
        canActivateChild: [AutoLoginAllRoutesGuard],
      },
      {
        path: 'general',
        component: GeneralComponent,
        canActivateChild: [AutoLoginAllRoutesGuard],
      },
    ],
  },
];

@NgModule({
  imports: [RouterModule.forChild(settingsRoutes)],
  exports: [RouterModule],
})
export class SettingsRoutingModule {}
