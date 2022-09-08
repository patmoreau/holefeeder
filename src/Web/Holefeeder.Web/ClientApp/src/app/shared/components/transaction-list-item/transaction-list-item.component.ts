import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';

@Component({
  selector: 'app-transaction-list-item',
  templateUrl: './transaction-list-item.component.html',
  styleUrls: ['./transaction-list-item.component.scss'],
})
export class TransactionListItemComponent implements OnInit {
  @Input() description!: string;
  @Input() amount!: number;
  @Input() date!: Date;
  @Input() tags!: string[];
  @Input() allowSave!: boolean;
  @Output() action: EventEmitter<string> = new EventEmitter<string>();

  constructor() {}

  ngOnInit() {}

  click(action: string) {
    this.action.emit(action);
  }
}
