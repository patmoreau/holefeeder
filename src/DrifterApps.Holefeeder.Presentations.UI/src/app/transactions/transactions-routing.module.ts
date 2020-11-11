import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { TransactionEditComponent } from './transaction-edit/transaction-edit.component';
import { MsalGuard } from '@azure/msal-angular';

const authRoutes: Routes = [
  {
    path: 'create',
    component: TransactionEditComponent,
    canActivate: [MsalGuard],
  },
  {
    path: ':transactionId',
    component: TransactionEditComponent,
    canActivate: [MsalGuard],
  }
];

@NgModule({
  imports: [RouterModule.forChild(authRoutes)],
  exports: [RouterModule]
})
export class TransactionsRoutingModule { }
