import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { MsalGuard } from '@azure/msal-angular';
import { PayCashflowComponent } from './pay-cashflow/pay-cashflow.component';
import { ModifyTransactionComponent } from './modify-transaction/modify-transaction.component';
import { MakePurchaseComponent } from './make-purchase/make-purchase.component';

const routes: Routes = [
  {
    path: 'pay-cashflow/:cashflowId',
    component: PayCashflowComponent,
    canActivate: [MsalGuard],
  },
  {
    path: 'make-purchase',
    component: MakePurchaseComponent,
    canActivate: [MsalGuard],
  },
  {
    path: 'make-purchase/:accountId',
    component: MakePurchaseComponent,
    canActivate: [MsalGuard],
  },
  {
    path: ':transactionId',
    component: ModifyTransactionComponent,
    canActivate: [MsalGuard],
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class TransactionsRoutingModule { }
