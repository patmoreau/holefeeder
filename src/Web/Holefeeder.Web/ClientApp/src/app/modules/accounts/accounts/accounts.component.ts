import { Component, OnInit } from '@angular/core';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-accounts',
  templateUrl: './accounts.component.html',
  styleUrls: ['./accounts.component.scss'],
  standalone: true,
  imports: [RouterModule],
})
export class AccountsComponent implements OnInit {
  constructor() {}

  ngOnInit() {}
}
