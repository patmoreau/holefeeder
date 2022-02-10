import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { TransactionsRoutingModule } from './transactions-routing.module';
import { SharedModule } from '@app/shared/shared.module';
import { TransactionEditComponent } from './transaction-edit/transaction-edit.component';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { PayCashflowComponent } from './pay-cashflow/pay-cashflow.component';
import { MakePurchaseComponent } from './make-purchase/make-purchase.component';
import { ModifyTransactionComponent } from './modify-transaction/modify-transaction.component';

@NgModule({
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    SharedModule,
    NgbModule,
    TransactionsRoutingModule,
  ],
  exports: [PayCashflowComponent, MakePurchaseComponent, ModifyTransactionComponent],
  declarations: [TransactionEditComponent, PayCashflowComponent, MakePurchaseComponent, ModifyTransactionComponent],
  providers: []
})
export class TransactionsModule { }
