import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { SettingsComponent } from './settings.component';
import { GeneralComponent } from './general/general.component';
import { AuthGuardService } from '@app/auth/services/auth-guard.service';
import { AccountComponent } from './account/account.component';

const settingsRoutes: Routes = [
  { path: '', redirectTo: '/settings/account', pathMatch: 'full' },
  {
    path: '',
    component: SettingsComponent,
    canActivate: [AuthGuardService],
    children: [
      { path: 'account', component: AccountComponent, canActivateChild: [AuthGuardService] },
      { path: 'general', component: GeneralComponent, canActivateChild: [AuthGuardService] }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(settingsRoutes)],
  exports: [RouterModule]
})
export class SettingsRoutingModule {}
