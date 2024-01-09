import { CommonModule, CurrencyPipe } from '@angular/common';
import { Component, OnInit, ViewChild, CUSTOM_ELEMENTS_SCHEMA, inject } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { PayCashflowCommandAdapter } from '@app/core/adapters';
import {
  MessageService,
  TransactionsService,
  UpcomingService,
} from '@app/core/services';
import {
  LoaderComponent,
  TransactionListItemComponent,
  TransactionsListComponent,
} from '@app/shared/components';
import { MessageAction, MessageType, Upcoming } from '@app/shared/models';
import { ChartConfiguration, ChartData, ChartType, ChartTypeRegistry, Color, LegendItem } from 'chart.js';
import { BaseChartDirective, NgChartsModule } from 'ng2-charts';
import { Observable, map, of } from 'rxjs';
import DatalabelsPlugin from 'chartjs-plugin-datalabels';
import { FilterNonNullishPipe } from '@app/shared/pipes';
import { filterNullish } from '@app/shared/helpers';
import { Store } from '@ngrx/store';
import { AppStore } from '@app/core/store';
import { StatisticsActions, StatisticsFeature } from '@app/core/store/statistics';

@Component({
  selector: 'app-dashboard-home',
  templateUrl: './dashboard-home.component.html',
  styleUrls: ['./dashboard-home.component.scss'],
  standalone: true,
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  providers: [CurrencyPipe],
  imports: [
    CommonModule,
    RouterModule,
    NgChartsModule,
    TransactionsListComponent,
    TransactionListItemComponent,
    LoaderComponent,
    FilterNonNullishPipe
  ]
})
export class DashboardHomeComponent implements OnInit {
  @ViewChild(BaseChartDirective) chart: BaseChartDirective | undefined;

  upcoming$!: Observable<Upcoming[]>;

  public chartData$: Observable<
    | ChartData<keyof ChartTypeRegistry, number[], string | string[] | unknown>
    | undefined
  > = of(undefined);

  private store = inject(Store<AppStore>);

  constructor(
    private upcomingService: UpcomingService,
    private transactionsService: TransactionsService,
    private adapter: PayCashflowCommandAdapter,
    private messages: MessageService,
    private router: Router,
    private currencyPipe: CurrencyPipe) { }

  ngOnInit(): void {
    this.upcoming$ = this.upcomingService.upcoming$;
    this.store.dispatch(StatisticsActions.loadSummary());

    this.chartData$ = this.store.select(StatisticsFeature.selectSummary)
      .pipe(
        filterNullish(),
        map(summary => {
          return {
            labels: ['Current', 'Average'],
            datasets: [
              {
                label: 'Gains',
                data: [summary.current.gains, summary.average.gains],
                backgroundColor: ['green'],
                stack: 'gains'
              },
              {
                label: 'Expenses',
                data: [summary.current.expenses, summary.average.expenses],
                backgroundColor: ['red'],
                stack: 'revenues'
              },
            ],
          };
        })
      );
  }

  async action(event: string, upcoming: Upcoming) {
    if (event === 'EDIT') {
      await this.router.navigate(
        ['transactions', 'pay-cashflow', upcoming.id],
        { queryParams: { date: upcoming.date } }
      );
    } else {
      this.transactionsService
        .payCashflow(
          this.adapter.adapt({
            date: upcoming.date,
            amount: upcoming.amount,
            cashflow: upcoming.id,
            cashflowDate: upcoming.date,
          })
        )
        .subscribe(id =>
          this.messages.sendMessage({
            type: MessageType.transaction,
            action: MessageAction.post,
            content: { id: id },
          })
        );
    }
  }

  // chart
  formatter = (value: string | number) => {
    return this.currencyPipe.transform(value);
  };

  public chartOptions: ChartConfiguration['options'] = {
    indexAxis: 'y',
    responsive: true,
    plugins: {
      tooltip: {
        enabled: false,
      },
      legend: {
        display: true,
        position: 'right',
        labels: {
          generateLabels: (chart): LegendItem[] => {
            return chart.data.datasets.map((dataset, i) => {
              return {
                datasetIndex: i,
                text: dataset.label || '',
                fillStyle: dataset.backgroundColor as Color || '',
              };
            });
          }
        }
      },
      datalabels: {
        color: 'white',
        formatter: this.formatter,
      },
    },
    scales: {
      x: {
        display: false,
        grid: {
          display: false,
        },
      },
      y: {
        display: false,
        grid: {
          display: false,
        },
      },
    },
  };

  public chartType: ChartType = 'bar';
  public chartPlugins = [DatalabelsPlugin];
}
