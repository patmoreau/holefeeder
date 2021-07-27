import { Component, OnInit } from '@angular/core';
import { StatisticsService } from '../services/statistics.service';
import { SettingsService } from '@app/singletons/services/settings.service';
import { CategoryType } from '@app/shared/enums/category-type.enum';
import { addMonths, startOfDay } from 'date-fns';
import { DateService } from '@app/singletons/services/date.service';
import { combineLatest, Subject } from 'rxjs';
import { Router } from '@angular/router';
import { ICategory } from '@app/shared/interfaces/category.interface';
import { CategoriesService } from '@app/shared/services/categories.service';
import { faDotCircle } from '@fortawesome/free-solid-svg-icons';

@Component({
  selector: 'dfta-statistics-categories',
  templateUrl: './statistics-categories.component.html',
  styleUrls: ['./statistics-categories.component.scss']
})
export class StatisticsCategoriesComponent implements OnInit {
  data: any[] = [];

  categories: ICategory[];

  faDotCircle = faDotCircle;

  constructor(
    private dateService: DateService,
    private categoriesService: CategoriesService,
    private statisticsService: StatisticsService,
    private settingsService: SettingsService,
    private router: Router) {}

  async ngOnInit() {
    this.categories = await this.categoriesService.find();

    combineLatest([
      this.settingsService.settings,
      this.dateService.period
    ])
      .subscribe(async ([settings, dateInterval]) => {
        const statistics = (await this.statisticsService.find(settings)).filter(i => i.item.type !== CategoryType.gain);

        const start = startOfDay(dateInterval.start);
        const end = startOfDay(dateInterval.end);
        const fromSixMonths = addMonths(start, -6);
        const fromTwelveMonths = addMonths(start, -12);

        const series = statistics
          .map(i => Object.assign({
            id: i.item.id,
            category: i.item.name,
            current: {
              count: i.period.filter(x => x.from >= start && x.to <= end).reduce((acc, item) => acc + item.count, 0),
              amount: i.period.filter(x => x.from >= start && x.to <= end).reduce((acc, item) => acc + item.amount, 0),
            },
            sixMonths: {
              count: i.monthly.filter(x => x.from >= fromSixMonths && x.to <= end).reduce((acc, item) => acc + item.count, 0),
              amount: i.monthly.filter(x => x.from >= fromSixMonths && x.to <= end).reduce((acc, item) => acc + item.amount, 0),
            },
            twelveMonths: {
              count: i.monthly.filter(x => x.from >= fromTwelveMonths && x.to <= end).reduce((acc, item) => acc + item.count, 0),
              amount: i.monthly.filter(x => x.from >= fromTwelveMonths && x.to <= end).reduce((acc, item) => acc + item.amount, 0),
            },
            avg: {
              count: Math.ceil(i.monthly.filter(x => x.from >= fromTwelveMonths && x.to <= end)
                .reduce((acc, item) => acc + item.count, 0) / 26),
              amount: i.monthly.filter(x => x.from >= fromTwelveMonths && x.to <= end).reduce((acc, item) => acc + item.amount, 0) / 26,
            },
          }))
          .filter(val => val.current.count > 0 || val.sixMonths.count > 0 || val.twelveMonths.count > 0)
          .sort((a: any, b: any) => b.avg.amount - a.avg.amount);

        this.data = [...series];
      });
  }

  getPercentage(category: any): number {
    const total = this.data.reduce((acc, item) => acc + item.avg.amount, 0);
    if (total > 0) {
      return category.avg.amount / total;
    }
    return 0;
  }

  getColorStyle(category: any): any {
    return { color: this.categories.filter(c => c.id === category.id)[0].color };
  }

  clickCategory(category: any) {
    this.router.navigate(['statistics', category.id]);
  }
}
