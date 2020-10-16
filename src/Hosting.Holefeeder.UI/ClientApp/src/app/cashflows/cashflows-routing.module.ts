import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AuthGuardService } from '@app/auth/services/auth-guard.service';
import { CashflowsListComponent } from './cashflows-list/cashflows-list.component';
import { CashflowEditComponent } from './cashflow-edit/cashflow-edit.component';
import { CashflowsComponent } from './cashflows/cashflows.component';

const routes: Routes = [
  {
    path: '',
    component: CashflowsComponent,
    children: [
      {
        path: '',
        component: CashflowsListComponent,
        canActivate: [AuthGuardService],
      },
      {
        path: 'create',
        component: CashflowEditComponent,
        canActivate: [AuthGuardService],
      },
      {
        path: ':cashflowId',
        component: CashflowEditComponent,
        canActivate: [AuthGuardService],
      },
    ]
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class CashflowsRoutingModule {}
