import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { CashflowsListComponent } from './cashflows-list/cashflows-list.component';
import { CashflowEditComponent } from './cashflow-edit/cashflow-edit.component';
import { CashflowsComponent } from './cashflows/cashflows.component';
import { MsalGuard } from '@azure/msal-angular';
const routes: Routes = [
  {
    path: '',
    component: CashflowsComponent,
    children: [
      {
        path: '',
        component: CashflowsListComponent,
        canActivate: [MsalGuard],
      },
      {
        path: 'create',
        component: CashflowEditComponent,
        canActivate: [MsalGuard],
      },
      {
        path: ':cashflowId',
        component: CashflowEditComponent,
        canActivate: [MsalGuard],
      },
    ]
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class CashflowsRoutingModule {}
