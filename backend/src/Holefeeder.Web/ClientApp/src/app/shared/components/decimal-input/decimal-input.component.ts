import { Component, Input } from '@angular/core';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { AutofocusDirective } from '../../directives';

@Component({
  selector: 'app-decimal-input',
  template: `
    <input
      appAutofocus
      class="form-control"
      [formControl]="control"
      [id]="id"
      [placeholder]="placeholder"
      [required]="required"
      type="number"
      inputmode="decimal"
      pattern="[0-9]*(.[0-9]{1,2})?" />
  `,
  standalone: true,
  imports: [AutofocusDirective, ReactiveFormsModule]
})
export class DecimalInputComponent {
  @Input() control = new FormControl();
  @Input() id = '';
  @Input() placeholder = '';
  @Input() required: boolean = false;
}
