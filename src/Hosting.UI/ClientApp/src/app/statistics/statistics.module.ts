import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';
import { StatisticsComponent } from './statistics/statistics.component';
import { StatisticsCategoriesComponent } from './statistics-categories/statistics-categories.component';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { StatisticsRoutingModule } from './statistics-routing.module';
import { StatisticsService } from './services/statistics.service';
import { SharedModule } from '@app/shared/shared.module';
import { NgxChartsModule } from '@swimlane/ngx-charts';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { StatisticsTagsComponent } from './statistics-tags/statistics-tags.component';

const COMPONENTS = [
  StatisticsComponent,
  StatisticsCategoriesComponent,
  StatisticsTagsComponent
];

@NgModule({
  declarations: [COMPONENTS],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    NgbModule,
    NgxChartsModule,
    FontAwesomeModule,
    SharedModule,
    StatisticsRoutingModule,
  ],
  providers: [StatisticsService],
  schemas: [CUSTOM_ELEMENTS_SCHEMA]
})
export class StatisticsModule { }
