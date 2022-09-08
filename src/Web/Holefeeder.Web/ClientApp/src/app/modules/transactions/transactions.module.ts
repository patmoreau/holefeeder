import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { TransferComponent } from '@app/modules/transactions/transfer/transfer.component';
import { SharedModule } from '@app/shared/shared.module';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { MakePurchaseComponent } from './make-purchase/make-purchase.component';
import { ModifyTransactionComponent } from './modify-transaction/modify-transaction.component';
import { PayCashflowComponent } from './pay-cashflow/pay-cashflow.component';
import { TransactionEditComponent } from './transaction-edit/transaction-edit.component';
import { TransactionsRoutingModule } from './transactions-routing.module';

@NgModule({
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    SharedModule,
    NgbModule,
    TransactionsRoutingModule,
  ],
  exports: [
    PayCashflowComponent,
    MakePurchaseComponent,
    ModifyTransactionComponent,
  ],
  declarations: [
    TransactionEditComponent,
    PayCashflowComponent,
    MakePurchaseComponent,
    ModifyTransactionComponent,
    TransferComponent,
  ],
  providers: [],
})
export class TransactionsModule {}
