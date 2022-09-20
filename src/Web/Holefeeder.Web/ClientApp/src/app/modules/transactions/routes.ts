import { Routes } from '@angular/router';
import { UpcomingResolverService } from '@app/core/resolvers/upcoming-resolver.service';
import { AutoLoginAllRoutesGuard } from 'angular-auth-oidc-client';

export const TRANSACTIONS_ROUTES: Routes = [
  {
    path: 'pay-cashflow/:cashflowId',
    loadComponent: () =>
      import('./pay-cashflow/pay-cashflow.component').then(
        m => m.PayCashflowComponent
      ),
    canActivate: [AutoLoginAllRoutesGuard],
    resolve: {
      cashflow: UpcomingResolverService,
    },
  },
  {
    path: 'make-purchase',
    loadComponent: () =>
      import('./make-purchase/make-purchase.component').then(
        m => m.MakePurchaseComponent
      ),
    canActivate: [AutoLoginAllRoutesGuard],
  },
  {
    path: 'make-purchase/:accountId',
    loadComponent: () =>
      import('./make-purchase/make-purchase.component').then(
        m => m.MakePurchaseComponent
      ),
    canActivate: [AutoLoginAllRoutesGuard],
  },
  {
    path: ':transactionId',
    loadComponent: () =>
      import('./modify-transaction/modify-transaction.component').then(
        m => m.ModifyTransactionComponent
      ),
    canActivate: [AutoLoginAllRoutesGuard],
  },
];
