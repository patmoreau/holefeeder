import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { CashflowDetail } from '@app/core/models/cashflow-detail.model';
import { CashflowsService } from '@app/core/services/cashflows.service';
import {
  LoaderComponent,
  TransactionListItemComponent,
} from '@app/shared/components';
import { TransactionsListComponent } from '@app/shared/components/transactions-list/transactions-list.component';
import { Observable, Subject } from 'rxjs';

@Component({
  selector: 'app-cashflows-list',
  templateUrl: './cashflows-list.component.html',
  styleUrls: ['./cashflows-list.component.scss'],
  standalone: true,
  imports: [
    CommonModule,
    TransactionsListComponent,
    TransactionListItemComponent,
    LoaderComponent,
  ],
})
export class CashflowsListComponent implements OnInit {
  cashflows$!: Observable<CashflowDetail[]>;
  showInactive = false;
  $showInactive = new Subject<boolean>();

  constructor(
    private cashflowsService: CashflowsService,
    private router: Router
  ) {}

  async ngOnInit(): Promise<void> {
    this.cashflows$ = this.cashflowsService.activeCashflows$;
  }

  inactiveChange() {
    this.showInactive = !this.showInactive;
    if (this.showInactive) {
      this.cashflows$ = this.cashflowsService.inactiveCashflows$;
    } else {
      this.cashflows$ = this.cashflowsService.activeCashflows$;
    }
  }

  click(id: string) {
    this.router.navigate(['/cashflows', id]);
  }
}
