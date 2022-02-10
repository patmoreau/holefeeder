import { Component, OnInit } from '@angular/core';
import { addMonths, startOfDay } from 'date-fns';
import { combineLatest, forkJoin, Observable, of, switchMap } from 'rxjs';
import { ActivatedRoute } from '@angular/router';
import { SettingsService } from '@app/core/services/settings.service';
import { CategoriesService } from '@app/core/services/categories.service';
import { Category } from '@app/core/models/category.model';
import { StatisticsService } from '@app/core/services/statistics.service';

@Component({
  selector: 'app-statistics-tags',
  templateUrl: './statistics-tags.component.html',
  styleUrls: ['./statistics-tags.component.scss']
})
export class StatisticsTagsComponent implements OnInit {
  data: any[] = [];

  category$!: Observable<Category>;

  isLoaded = false;

  constructor(
    private categoriesService: CategoriesService,
    private statisticsService: StatisticsService,
    private settingsService: SettingsService,
    private activatedRoute: ActivatedRoute) { }

  ngOnInit() {
    const id = this.activatedRoute.snapshot.paramMap.get('categoryId');
    this.category$ = this.categoriesService.findOneById(id!);

    combineLatest([
      this.category$,
      this.settingsService.settings$,
      this.settingsService.period$,
    ])
      .pipe(
        switchMap(([category, settings, dateInterval]) => forkJoin({ statistics: this.statisticsService.findByCategoryId(category.id, settings), dateInterval: of(dateInterval) }))
      )
      .subscribe(stats => {
        const start = startOfDay(stats.dateInterval.start);
        const end = startOfDay(stats.dateInterval.end);
        const fromSixMonths = addMonths(start, -6);
        const fromTwelveMonths = addMonths(start, -12);

        const series = stats.statistics
          .map(i => Object.assign({
            tag: i.item,
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

        this.isLoaded = true;
      });
  }

  getPercentage(tag: any): number {
    const total = this.data.reduce((acc, item) => acc + item.avg.amount, 0);
    if (total > 0) {
      return tag.avg.amount / total;
    }
    return 0;
  }
}
