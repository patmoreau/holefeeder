import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { TransactionsRoutingModule } from './transactions-routing.module';
import { SharedModule } from '@app/shared/shared.module';
import { TransactionEditComponent } from './transaction-edit/transaction-edit.component';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { TransactionsService } from './services/transactions.service';
import { TransactionsApiService } from './services/api/transactions-api.service';
import { TransactionDetailAdapter } from './models/transaction-detail.model';
import { PayCashflowComponent } from './pay-cashflow/pay-cashflow.component';
import { MakePurchaseComponent } from './make-purchase/make-purchase.component';
import { ModifyTransactionComponent } from './modify-transaction/modify-transaction.component';
import { MakePurchaseCommandAdapter } from './models/make-purchase-command.model';
import { ModifyTransactionCommandAdapter } from './models/modify-transaction-command.model';
import { PayCashflowCommandAdapter } from './models/pay-cashflow-command.model';

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
  providers: [TransactionsService, TransactionsApiService, TransactionDetailAdapter, MakePurchaseCommandAdapter, ModifyTransactionCommandAdapter, PayCashflowCommandAdapter]
})
export class TransactionsModule { }
