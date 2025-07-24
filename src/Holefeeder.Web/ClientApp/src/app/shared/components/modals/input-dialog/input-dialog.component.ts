import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { FormControl, ReactiveFormsModule, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-input-dialog',
  template: ` <div>
    <div class="modal-header">
      <h4 class="modal-title">{{ title }}</h4>
    </div>
    <div class="modal-body">
      <p>{{ message }}</p>
      <input [formControl]="input" type="text" />
    </div>
    <div class="modal-footer">
      <button
        type="button"
        class="btn btn-outline-dark"
        (click)="activeModal.close()">
        Cancel
      </button>
      <button
        type="button"
        class="btn btn-outline-dark"
        [class.disabled]="input.invalid"
        [disabled]="input.invalid"
        (click)="activeModal.close(input.value)">
        OK
      </button>
    </div>
  </div>`,
  changeDetection: ChangeDetectionStrategy.OnPush,
  standalone: true,
  imports: [ReactiveFormsModule]
})
export class InputDialogComponent {
  activeModal = inject(NgbActiveModal);

  title!: string;
  message!: string;
  input = new FormControl('', Validators.required);

  set initialValue(value: string) {
    this.input.setValue(value);
  }
}
