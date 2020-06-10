import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { SettingsComponent } from './settings.component';
import { GeneralComponent } from './general/general.component';
import { AccountComponent } from './account/account.component';
import { MsalGuard } from '@azure/msal-angular';

const settingsRoutes: Routes = [
  { path: '', redirectTo: '/settings/account', pathMatch: 'full' },
  {
    path: '',
    component: SettingsComponent,
    canActivate: [MsalGuard],
    children: [
      { path: 'account', component: AccountComponent, canActivateChild: [MsalGuard] },
      { path: 'general', component: GeneralComponent, canActivateChild: [MsalGuard] }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(settingsRoutes)],
  exports: [RouterModule]
})
export class SettingsRoutingModule {}
