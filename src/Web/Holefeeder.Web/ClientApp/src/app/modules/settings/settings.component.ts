import { Component, OnInit } from '@angular/core';
import { RouterModule } from '@angular/router';

@Component({
  templateUrl: './settings.component.html',
  styleUrls: ['./settings.component.scss'],
  standalone: true,
  imports: [RouterModule],
})
export class SettingsComponent implements OnInit {
  constructor() {}

  ngOnInit() {}
}
