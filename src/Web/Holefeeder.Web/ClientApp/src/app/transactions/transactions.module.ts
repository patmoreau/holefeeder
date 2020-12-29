import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { TransactionsRoutingModule } from './transactions-routing.module';
import { SharedModule } from '@app/shared/shared.module';
import { TransactionEditComponent } from './transaction-edit/transaction-edit.component';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';

const COMPONENTS = [TransactionEditComponent];

@NgModule({
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    SharedModule,
    FontAwesomeModule,
    NgbModule,
    TransactionsRoutingModule,
  ],
  exports: [COMPONENTS],
  declarations: [COMPONENTS]
})
export class TransactionsModule { }
