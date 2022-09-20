import { Component, OnInit } from '@angular/core';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-cashflows',
  templateUrl: './cashflows.component.html',
  styleUrls: ['./cashflows.component.scss'],
  standalone: true,
  imports: [RouterModule],
})
export class CashflowsComponent implements OnInit {
  constructor() {}

  ngOnInit() {}
}
