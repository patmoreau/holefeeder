import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { TransactionEditComponent } from './transaction-edit/transaction-edit.component';
import { AuthGuardService } from '@app/auth/services/auth-guard.service';

const authRoutes: Routes = [
  {
    path: 'create',
    component: TransactionEditComponent,
    canActivate: [AuthGuardService],
  },
  {
    path: ':transactionId',
    component: TransactionEditComponent,
    canActivate: [AuthGuardService],
  }
];

@NgModule({
  imports: [RouterModule.forChild(authRoutes)],
  exports: [RouterModule]
})
export class TransactionsRoutingModule { }
