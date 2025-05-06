import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { CashflowsService } from '@app/core/services';
import {
  LoaderComponent,
  TransactionListItemComponent,
} from '@app/shared/components';
import { CashflowDetail } from '@app/shared/models';
import { Observable, Subject } from 'rxjs';

@Component({
  selector: 'app-cashflows-list',
  templateUrl: './cashflows-list.component.html',
  styleUrls: ['./cashflows-list.component.scss'],
  standalone: true,
  imports: [
    CommonModule,
    TransactionListItemComponent,
    LoaderComponent
  ]
})
export class CashflowsListComponent implements OnInit {
  cashflows$!: Observable<CashflowDetail[]>;
  showInactive = false;
  $showInactive = new Subject<boolean>();

  constructor(
    private cashflowsService: CashflowsService,
    private router: Router
  ) { }

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
