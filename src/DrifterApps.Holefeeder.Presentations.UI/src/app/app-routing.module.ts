import { NgModule } from '@angular/core';
import { Routes, RouterModule, ExtraOptions } from '@angular/router';
import { ErrorNotfoundComponent } from './error-notfound/error-notfound.component';
import { MsalGuard } from '@azure/msal-angular';

const appRoutes: Routes = [
  { path: '', redirectTo: '/dashboard', pathMatch: 'full' },
  {
    path: 'dashboard',
    loadChildren: () => import('./dashboard/dashboard.module').then(m => m.DashboardModule), canActivate: [MsalGuard]
  },
  {
    path: 'accounts',
    loadChildren: () => import('./accounts/accounts.module').then(m => m.AccountsModule), canActivate: [MsalGuard]
  },
  {
    path: 'settings',
    loadChildren: () => import('./settings/settings.module').then(m => m.SettingsModule), canActivate: [MsalGuard]
  },
  {
    path: 'cashflows',
    loadChildren: () => import('./cashflows/cashflows.module').then(m => m.CashflowsModule), canActivate: [MsalGuard]
  },
  {
    path: 'transactions',
    loadChildren: () => import('./transactions/transactions.module').then(m => m.TransactionsModule), canActivate: [MsalGuard]
  },
  {
    path: 'statistics',
    loadChildren: () => import('./statistics/statistics.module').then(m => m.StatisticsModule), canActivate: [MsalGuard]
  },
  { path: 'oauthcallback', redirectTo: '/dashboard', pathMatch: 'full' },
  { path: '**', component: ErrorNotfoundComponent }
];

@NgModule({
  imports: [
    RouterModule.forRoot(appRoutes)
  ],
  exports: [RouterModule]
})
export class AppRoutingModule { }
