import {NgModule} from '@angular/core';
import {RouterModule, Routes} from '@angular/router';
import {CashflowsListComponent} from './cashflows-list/cashflows-list.component';
import {ModifyCashflowComponent} from './modify-cashflow/modify-cashflow.component';
import {CashflowsComponent} from './cashflows/cashflows.component';
import {MsalGuard} from '@azure/msal-angular';

const routes: Routes = [
  {
    path: '',
    component: CashflowsComponent,
    children: [
      {
        path: '',
        component: CashflowsListComponent,
        canActivate: [MsalGuard],
        runGuardsAndResolvers: 'always',
        pathMatch: 'full'
      },
      {
        path: ':cashflowId',
        component: ModifyCashflowComponent,
        canActivate: [MsalGuard],
      },
    ]
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class CashflowsRoutingModule {
}
