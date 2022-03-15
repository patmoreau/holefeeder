import {Component, Input, OnInit} from '@angular/core';

@Component({
  selector: 'app-date-view',
  templateUrl: './date-view.component.html',
  styleUrls: ['./date-view.component.scss']
})
export class DateViewComponent implements OnInit {
  @Input() date: Date | undefined;
  @Input() color: string | undefined;

  constructor() {
  }

  ngOnInit() {
  }

}
