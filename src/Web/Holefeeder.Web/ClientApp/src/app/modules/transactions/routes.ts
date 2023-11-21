import { Routes } from '@angular/router';
import { UpcomingResolverService } from '@app/core/resolvers';
import { authenticationGuard } from "@app/core/auth/authorization.guard";

export const TRANSACTIONS_ROUTES: Routes = [
  {
    path: 'pay-cashflow/:cashflowId',
    loadComponent: () =>
      import('./pay-cashflow/pay-cashflow.component').then(
        m => m.PayCashflowComponent
      ),
    canActivate: [authenticationGuard],
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
    canActivate: [authenticationGuard],
  },
  {
    path: 'make-purchase/:accountId',
    loadComponent: () =>
      import('./make-purchase/make-purchase.component').then(
        m => m.MakePurchaseComponent
      ),
    canActivate: [authenticationGuard],
  },
  {
    path: ':transactionId',
    loadComponent: () =>
      import('./modify-transaction/modify-transaction.component').then(
        m => m.ModifyTransactionComponent
      ),
    canActivate: [authenticationGuard],
  },
];
