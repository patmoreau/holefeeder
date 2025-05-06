import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'app-transaction-list-item',
  templateUrl: './transaction-list-item.component.html',
  styleUrls: ['./transaction-list-item.component.scss'],
  standalone: true,
  imports: [CommonModule]
})
export class TransactionListItemComponent {
  @Input() description!: string;
  @Input() amount!: number;
  @Input() date!: Date;
  @Input() tags!: string[];
  @Input() allowSave!: boolean;
  @Output() action: EventEmitter<string> = new EventEmitter<string>();

  click(action: string) {
    this.action.emit(action);
  }
}
