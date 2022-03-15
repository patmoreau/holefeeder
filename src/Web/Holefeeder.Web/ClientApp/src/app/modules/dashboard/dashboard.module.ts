import {CUSTOM_ELEMENTS_SCHEMA, NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import {DashboardRoutingModule} from './dashboard-routing.module';
import {SharedModule} from '@app/shared/shared.module';
import {DashboardHomeComponent} from './dashboard-home/dashboard-home.component';
import {DashboardComponent} from './dashboard/dashboard.component';

const COMPONENTS = [
  DashboardComponent,
  DashboardHomeComponent
];

@NgModule({
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    SharedModule,
    DashboardRoutingModule
  ],
  declarations: [COMPONENTS],
  providers: [],
  schemas: [CUSTOM_ELEMENTS_SCHEMA]
})
export class DashboardModule {
}
