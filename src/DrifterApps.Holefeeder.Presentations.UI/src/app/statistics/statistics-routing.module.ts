import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { StatisticsComponent } from './statistics/statistics.component';
import { StatisticsCategoriesComponent } from './statistics-categories/statistics-categories.component';
import { StatisticsTagsComponent } from './statistics-tags/statistics-tags.component';

const routes: Routes = [
  {
    path: '',
    component: StatisticsComponent,
    children: [
      {
        path: '',
        component: StatisticsCategoriesComponent,
        pathMatch: 'full'
      },
      {
        path: ':categoryId',
        component: StatisticsTagsComponent,
      }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class StatisticsRoutingModule { }
