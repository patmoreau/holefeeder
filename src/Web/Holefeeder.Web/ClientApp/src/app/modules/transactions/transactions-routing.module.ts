import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { UpcomingResolverService } from '@app/core/resolvers/upcoming-resolver.service';
import { AutoLoginAllRoutesGuard } from 'angular-auth-oidc-client';
import { MakePurchaseComponent } from './make-purchase/make-purchase.component';
import { ModifyTransactionComponent } from './modify-transaction/modify-transaction.component';
import { PayCashflowComponent } from './pay-cashflow/pay-cashflow.component';

const routes: Routes = [
  {
    path: 'pay-cashflow/:cashflowId',
    component: PayCashflowComponent,
    canActivate: [AutoLoginAllRoutesGuard],
    resolve: {
      cashflow: UpcomingResolverService,
    },
  },
  {
    path: 'make-purchase',
    component: MakePurchaseComponent,
    canActivate: [AutoLoginAllRoutesGuard],
  },
  {
    path: 'make-purchase/:accountId',
    component: MakePurchaseComponent,
    canActivate: [AutoLoginAllRoutesGuard],
  },
  {
    path: ':transactionId',
    component: ModifyTransactionComponent,
    canActivate: [AutoLoginAllRoutesGuard],
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class TransactionsRoutingModule {}
