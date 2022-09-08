import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { BrowserUtils } from '@azure/msal-browser';
import { ErrorNotfoundComponent } from './core/error-notfound/error-notfound.component';

const routes: Routes = [
  { path: '', redirectTo: '/dashboard', pathMatch: 'full' },
  {
    path: 'dashboard',
    loadChildren: () =>
      import('./modules/dashboard/dashboard.module').then(
        m => m.DashboardModule
      ),
  },
  {
    path: 'accounts',
    loadChildren: () =>
      import('./modules/accounts/accounts.module').then(m => m.AccountsModule),
  },
  {
    path: 'settings',
    loadChildren: () =>
      import('./modules/settings/settings.module').then(m => m.SettingsModule),
  },
  {
    path: 'cashflows',
    loadChildren: () =>
      import('./modules/cashflows/cashflows.module').then(
        m => m.CashflowsModule
      ),
  },
  {
    path: 'transactions',
    loadChildren: () =>
      import('./modules/transactions/transactions.module').then(
        m => m.TransactionsModule
      ),
  },
  { path: '**', component: ErrorNotfoundComponent },
];

@NgModule({
  imports: [
    RouterModule.forRoot(routes, {
      initialNavigation:
        !BrowserUtils.isInIframe() && !BrowserUtils.isInPopup()
          ? 'enabledBlocking'
          : 'disabled',
    }),
  ],
  exports: [RouterModule],
})
export class AppRoutingModule {}
