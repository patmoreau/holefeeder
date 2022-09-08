import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { MsalGuard } from '@azure/msal-angular';
import { AccountComponent } from './account/account.component';
import { GeneralComponent } from './general/general.component';
import { SettingsComponent } from './settings.component';

const settingsRoutes: Routes = [
  { path: '', redirectTo: '/settings/account', pathMatch: 'full' },
  {
    path: '',
    component: SettingsComponent,
    canActivate: [MsalGuard],
    children: [
      {
        path: 'account',
        component: AccountComponent,
        canActivateChild: [MsalGuard],
      },
      {
        path: 'general',
        component: GeneralComponent,
        canActivateChild: [MsalGuard],
      },
    ],
  },
];

@NgModule({
  imports: [RouterModule.forChild(settingsRoutes)],
  exports: [RouterModule],
})
export class SettingsRoutingModule {}
