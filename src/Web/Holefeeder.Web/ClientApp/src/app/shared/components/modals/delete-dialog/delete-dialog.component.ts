import { ChangeDetectionStrategy, Component } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-confirm-dialog',
  template: ` <div class="modal-header">
      <h4 class="modal-title text-danger" id="modal-delete-title">
        {{ title }}
      </h4>
      <button
        (click)="activeModal.close(false)"
        aria-label="Close"
        class="close"
        type="button">
        <span aria-hidden="true">&times;</span>
      </button>
    </div>
    <div class="modal-body">
      <strong>{{ prompt }}</strong>
    </div>
    <div class="modal-footer">
      <button
        (click)="activeModal.close(false)"
        class="btn btn-secondary"
        ngbAutofocus
        type="button">
        Cancel
      </button>
      <button
        (click)="activeModal.close(true)"
        class="btn btn-danger"
        type="button">
        {{ action }}
      </button>
    </div>`,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class DeleteDialogComponent {
  title!: string;
  prompt!: string;
  action!: string;

  constructor(public activeModal: NgbActiveModal) {}
}
