import { CommonModule } from '@angular/common';
import { CUSTOM_ELEMENTS_SCHEMA, NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SharedModule } from '@app/shared/shared.module';
import { DashboardHomeComponent } from './dashboard-home/dashboard-home.component';
import { DashboardRoutingModule } from './dashboard-routing.module';
import { DashboardComponent } from './dashboard/dashboard.component';

const COMPONENTS = [DashboardComponent, DashboardHomeComponent];

@NgModule({
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    SharedModule,
    DashboardRoutingModule,
  ],
  declarations: [COMPONENTS],
  providers: [],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
})
export class DashboardModule {}
