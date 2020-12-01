import { Component, OnInit, ViewEncapsulation, Input } from '@angular/core';

@Component({
  selector: 'dfta-date-view',
  templateUrl: './date-view.component.html',
  styleUrls: ['./date-view.component.scss']
})
export class DateViewComponent implements OnInit {
  @Input() date: Date;
  @Input() color: string;
  constructor() { }

  ngOnInit() {
  }

}
