import { ChangeDetectionStrategy, Component } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-confirm-dialog',
  template: ` <div>
    <div class="modal-header">
      <h4 class="modal-title">{{ title }}</h4>
    </div>
    <div class="modal-body">
      <p>{{ prompt }}</p>
    </div>
    <div class="modal-footer">
      <button
        type="button"
        class="btn btn-outline-dark"
        (click)="activeModal.close(false)">
        Cancel
      </button>
      <button
        type="button"
        class="btn btn-outline-dark"
        (click)="activeModal.close(true)">
        OK
      </button>
    </div>
  </div>`,
  changeDetection: ChangeDetectionStrategy.OnPush,
  standalone: true,
})
export class ConfirmDialogComponent {
  title: string = 'Title placeholder';
  prompt: string = 'Prompt placeholder';

  constructor(public activeModal: NgbActiveModal) {}
}
