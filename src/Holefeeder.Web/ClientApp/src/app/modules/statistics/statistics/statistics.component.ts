import { CommonModule, CurrencyPipe } from '@angular/common';
import {
  Component,
  CUSTOM_ELEMENTS_SCHEMA,
  OnInit,
  ViewChild,
} from '@angular/core';
import { RouterModule } from '@angular/router';
import { map, Observable, of } from 'rxjs';
import { Statistics } from '@app/shared/models';
import { StatisticsService } from '@app/core/services';
import { BaseChartDirective } from 'ng2-charts';
import {
  ChartConfiguration,
  ChartData,
  ChartType,
  ChartTypeRegistry,
  TooltipItem,
} from 'chart.js';
import { LoaderComponent } from '@app/shared/components';
import { filterNullish } from '@app/shared/helpers';
import DatalabelsPlugin from 'chartjs-plugin-datalabels';
import { FilterNonNullishPipe } from '@app/shared/pipes';

// noinspection ES6ClassMemberInitializationOrder
@Component({
    selector: 'app-statistics',
    templateUrl: './statistics.component.html',
    styleUrls: ['./statistics.component.scss'],
    imports: [
        CommonModule,
        RouterModule,
        BaseChartDirective,
        LoaderComponent,
        FilterNonNullishPipe,
    ],
    schemas: [CUSTOM_ELEMENTS_SCHEMA],
    providers: [CurrencyPipe]
})
export class StatisticsComponent implements OnInit {
  @ViewChild(BaseChartDirective) chart: BaseChartDirective | undefined;

  public statistics$!: Observable<Statistics[]>;

  public chartData$: Observable<
    | ChartData<keyof ChartTypeRegistry, number[], string | string[] | unknown>
    | undefined
  > = of(undefined);
  constructor(
    private currencyPipe: CurrencyPipe,
    private statisticsService: StatisticsService
  ) {}
  ngOnInit(): void {
    this.statistics$ = this.statisticsService.find();

    this.chartData$ = this.statistics$.pipe(
      filterNullish(),
      map((array: Statistics[]) => {
        return {
          labels: array.map(item => item.category),
          datasets: [
            {
              data: array.map(item => item.monthlyAverage),
              backgroundColor: array.map(item => item.color),
            },
          ],
        };
      }),
      filterNullish()
    );
  }

  footer = (tooltipItems: { parsed: { y: number } }[]) => {
    // const dataset = data.datasets[tooltipItem.datasetIndex];
    // const value = dataset.data[tooltipItem.index];
    // const total = dataset.data.reduce((acc: any, curr: any) => acc + curr, 0);
    // const percentage = ((value / total) * 100).toFixed(2);
    // return `${dataset.label}: ${percentage}%`;
    let sum = 0;

    tooltipItems.forEach(function (tooltipItem: { parsed: { y: number } }) {
      sum += tooltipItem.parsed.y;
    });
    return 'Sum: ' + sum;
  };

  label = (tooltipItem: TooltipItem<keyof ChartTypeRegistry>) => {
    const context = tooltipItem as {
      dataset: { data: number[] };
      dataIndex: number;
    };
    const value = context.dataset.data[context.dataIndex] as number;
    const total: number = context.dataset.data.reduce(
      (acc: number, curr: number) => acc + curr,
      0
    ) as number;
    const percentage = ((value / total) * 100).toFixed(2);
    return `${this.currencyPipe.transform(value)} (${percentage}%)`;
  };

  formatter = (value: string | number) => {
    return this.currencyPipe.transform(value);
  };

  // Pie
  public pieChartOptions: ChartConfiguration['options'] = {
    responsive: true,
    plugins: {
      tooltip: {
        callbacks: {
          footer: this.footer,
          label: this.label,
        },
      },
      legend: {
        display: true,
        position: 'bottom',
      },
      datalabels: {
        formatter: this.formatter,
      },
    },
  };

  public getChartData(
    data: {
      labels?: string[];
      data?: number[];
      colors?: string[];
    } | null
  ): ChartData<'pie', number[], string | string[]> {
    console.log(data);
    if (!data) {
      return this.chartData;
    }
    this.chartData = {
      labels: data.labels ?? [],
      datasets: [
        {
          data: data.data ?? [],
          backgroundColor: data.colors ?? [],
        },
      ],
    };
    return this.chartData;
  }
  private chartData: ChartData<'pie', number[], string | string[]> = {
    labels: [],
    datasets: [
      {
        data: [],
        backgroundColor: [],
      },
    ],
  };
  public pieChartType: ChartType = 'pie';
  public pieChartPlugins = [DatalabelsPlugin];
}
