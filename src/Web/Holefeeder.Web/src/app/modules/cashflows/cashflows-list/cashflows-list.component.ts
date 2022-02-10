import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Observable, Subject } from 'rxjs';
import { CashflowsService } from '@app/core/services/cashflows.service';
import { CashflowDetail } from '@app/core/models/cashflow-detail.model';
import { PagingInfo } from '@app/core/models/paging-info.model';

@Component({
  selector: 'app-cashflows-list',
  templateUrl: './cashflows-list.component.html',
  styleUrls: ['./cashflows-list.component.scss']
})
export class CashflowsListComponent implements OnInit {
  cashflows$!: Observable<PagingInfo<CashflowDetail>>;
  showInactive = false;
  $showInactive = new Subject<boolean>();

  constructor(private cashflowsService: CashflowsService, private router: Router) { }

  async ngOnInit(): Promise<void> {
    this.$showInactive.subscribe(async (showInactive) => {
      this.cashflows$ = this.cashflowsService.find(0, 0, [
        'description'
      ], [
        showInactive ? 'inactive:eq:true' : 'inactive:eq:false'
      ]);
    });
    this.$showInactive.next(this.showInactive);
  }

  inactiveChange() {
    this.showInactive = !this.showInactive;
    this.$showInactive.next(this.showInactive);
  }

  click(id: string) {
    this.router.navigate(['/cashflows', id]);
  }
}
