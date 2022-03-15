import {CUSTOM_ELEMENTS_SCHEMA, NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {CashflowsListComponent} from './cashflows-list/cashflows-list.component';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import {NgbModule} from '@ng-bootstrap/ng-bootstrap';
import {SharedModule} from '@app/shared/shared.module';
import {CashflowsRoutingModule} from './cashflows-routing.module';
import {CashflowEditComponent} from './cashflow-edit/cashflow-edit.component';
import {CashflowsComponent} from './cashflows/cashflows.component';

const COMPONENTS = [
  CashflowsListComponent,
  CashflowEditComponent,
  CashflowsComponent
];

@NgModule({
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    NgbModule,
    SharedModule,
    CashflowsRoutingModule
  ],
  declarations: [COMPONENTS],
  schemas: [CUSTOM_ELEMENTS_SCHEMA]
})
export class CashflowsModule {
}
