import { Component, OnInit } from '@angular/core';
import { ICashflow } from '@app/shared/interfaces/cashflow.interface';
import { CashflowsService } from '../../shared/services/cashflows.service';
import { Router } from '@angular/router';
import { Subject } from 'rxjs';
import { faPlus } from '@fortawesome/free-solid-svg-icons';

@Component({
  selector: 'dfta-cashflows-list',
  templateUrl: './cashflows-list.component.html',
  styleUrls: ['./cashflows-list.component.scss']
})
export class CashflowsListComponent implements OnInit {
  cashflows: ICashflow[];
  showInactive = false;
  $showInactive = new Subject<boolean>();

  faPlus = faPlus;

  constructor(private cashflowsService: CashflowsService, private router: Router) { }

  async ngOnInit(): Promise<void> {
    this.$showInactive.subscribe(async (showInactive) => {
      this.cashflows = await this.cashflowsService.find(null, null, [
        'description'
      ], [
        showInactive ? 'inactive=true' : 'inactive!=true'
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
