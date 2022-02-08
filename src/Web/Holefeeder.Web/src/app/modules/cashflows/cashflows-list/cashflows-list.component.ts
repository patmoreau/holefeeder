import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Subject } from 'rxjs';
import { ICashflowDetail } from '@app/shared/interfaces/cashflow-detail.interface';
import { PagingInfo } from '@app/shared/interfaces/paging-info.interface';
import { CashflowsService } from '@app/shared/services/cashflows.service';

@Component({
  selector: 'dfta-cashflows-list',
  templateUrl: './cashflows-list.component.html',
  styleUrls: ['./cashflows-list.component.scss']
})
export class CashflowsListComponent implements OnInit {
  cashflows: PagingInfo<ICashflowDetail>;
  showInactive = false;
  $showInactive = new Subject<boolean>();

  constructor(private cashflowsService: CashflowsService, private router: Router) { }

  async ngOnInit(): Promise<void> {
    this.$showInactive.subscribe(async (showInactive) => {
      this.cashflows = await this.cashflowsService.find(null, null, [
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
