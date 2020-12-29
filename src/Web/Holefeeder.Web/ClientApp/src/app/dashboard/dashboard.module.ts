import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { DashboardRoutingModule } from './dashboard-routing.module';
import { SharedModule } from '@app/shared/shared.module';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { DashboardHomeComponent } from './dashboard-home/dashboard-home.component';
import { DashboardComponent } from './dashboard/dashboard.component';

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
    FontAwesomeModule,
    DashboardRoutingModule
  ],
  declarations: [COMPONENTS],
  providers: [],
  schemas: [CUSTOM_ELEMENTS_SCHEMA]
})
export class DashboardModule { }
