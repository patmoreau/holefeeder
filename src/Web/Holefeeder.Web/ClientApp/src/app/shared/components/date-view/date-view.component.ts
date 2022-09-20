import { CommonModule } from '@angular/common';
import { Component, Input, OnInit } from '@angular/core';
import { NgbDatepickerModule } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-date-view',
  templateUrl: './date-view.component.html',
  styleUrls: ['./date-view.component.scss'],
  standalone: true,
  imports: [CommonModule, NgbDatepickerModule],
})
export class DateViewComponent implements OnInit {
  @Input() date: Date | undefined;
  @Input() color: string | undefined;

  constructor() {}

  ngOnInit() {}
}
