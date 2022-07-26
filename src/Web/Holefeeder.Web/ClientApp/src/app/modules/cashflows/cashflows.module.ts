import {CUSTOM_ELEMENTS_SCHEMA, NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {CashflowsListComponent} from './cashflows-list/cashflows-list.component';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import {SharedModule} from '@app/shared/shared.module';
import {CashflowsRoutingModule} from './cashflows-routing.module';
import {ModifyCashflowComponent} from './modify-cashflow/modify-cashflow.component';
import {CashflowsComponent} from './cashflows/cashflows.component';

const COMPONENTS = [
  CashflowsListComponent,
  ModifyCashflowComponent,
  CashflowsComponent
];

@NgModule({
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    SharedModule,
    CashflowsRoutingModule
  ],
  declarations: [COMPONENTS],
  schemas: [CUSTOM_ELEMENTS_SCHEMA]
})
export class CashflowsModule {
}
